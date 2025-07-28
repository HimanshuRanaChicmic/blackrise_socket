using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Exceptions;
using BlackRise.Identity.Application.Feature.Login;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Application.Settings;
using BlackRise.Identity.Domain;
using BlackRise.Identity.Domain.Common.Enums;
using BlackRise.Identity.Persistence.Settings;
using BlackRise.Identity.Application.Utils;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlackRise.Identity.Application.Helpers;

namespace BlackRise.Identity.Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JwtSetting _jwtSettings;
    private readonly AppleSetting _appleSetting;
    private readonly ClientUrlSetting _clientUrlSettings;
    private readonly LinkedInSetting _linkedInSetting;
    private readonly GoogleSetting _googleSetting;
    private readonly IHttpWrapper _httpWrapper;
    private readonly ILogger<AuthService> _logger;
    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSetting> jwtSettings,
        IOptions<AppleSetting> appleSetting,
        IOptions<ClientUrlSetting> clientUrlSettings, IHttpWrapper httpWrapper, 
        IOptions<LinkedInSetting> linkedInSetting, ILogger<AuthService> logger,
        IOptions<GoogleSetting> googleSetting) 
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _clientUrlSettings = clientUrlSettings.Value;
        _httpWrapper = httpWrapper;
        _jwtSettings = jwtSettings.Value;
        _linkedInSetting = linkedInSetting.Value;
        _googleSetting = googleSetting.Value;
        _appleSetting = appleSetting.Value;
        _logger = logger;
    }

    public async Task<LoginDto> LoginAsync(string username, string password)
    {
        _logger.LogInformation($"Simple Login");
        var user = await _userManager.FindByEmailAsync(username);

        if (user == null || user.IsDeleted)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidEmailPassword));

        if(!user.EmailConfirmed)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailNotConfirmed));

        if(user.IsSocialLogin)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserAlreadyRegisteredWithSocialLogin));

        if (user.PasswordHash == null && !user.IsSocialLogin)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserPasswordNotSet));

        if (!user.IsActive)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserAccountDisabled));

        var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

        if (result.IsNotAllowed)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailNotConfirmed));

        if (!result.Succeeded)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUsernamePassword));

        string token = await GenerateTokenAsync(user);

        var loginDto = new LoginDto(token)
        {
            Email = user.Email,
            UserId = user.Id,
            AppleId = user.AppleId,
            IsProfileCreated = user.IsProfileCreated,
            IsProfileCompleted = user.IsProfileCompleted
        };
        return loginDto;
    }

    public async Task<string> RegisterAsync(string username, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(username);

        if (existingUser != null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailAlreadyRegistered));

        var newUser = new ApplicationUser 
        { 
            UserName = username,
            NormalizedUserName = username.ToUpper(),
            Email = username, 
            NormalizedEmail = username.ToUpper(),
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
            IsActive = true,
            EmailConfirmed = false,
        };
        newUser.CreatedBy = newUser.Id;
        newUser.ModifiedBy = newUser.Id;

        var result = await _userManager.CreateAsync(newUser, password);

        if (!result.Succeeded)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.ErrorCreatingUser));

        _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());

        await SendEmailConfirmationCodeAsync(newUser);

        return LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
    }

    public async Task<string> RegisterAsync(SignupCommand signupCommand)
    {
        var existingUser = await _userManager.FindByEmailAsync(signupCommand.Email);
        var otp = "";

        if (existingUser != null && existingUser.IsSocialLogin)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserAlreadyRegisteredWithSocialLogin));

        if (existingUser != null && existingUser.EmailConfirmed && !existingUser.IsSocialLogin && existingUser.PasswordHash != null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailAlreadyRegistered));

        if (existingUser != null)
        {
            if (existingUser.IsSocialLogin && !existingUser.IsProfileCreated)
            {
                await CreateProfileAsync(existingUser, signupCommand);
                return LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.ProfileCreatedSuccessfully);
            }
            else
            {
                await SendEmailConfirmationCodeAsync(existingUser);
                await UpdateProfileAsync(existingUser, signupCommand);
            }
            otp = existingUser.EmailConfirmationCode;
        }
        else
        {
            var newUser = new ApplicationUser
            {
                UserName = signupCommand.Email,
                NormalizedUserName = signupCommand.Email.ToUpper(),
                Email = signupCommand.Email,
                NormalizedEmail = signupCommand.Email.ToUpper(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IsDeleted = false,
                IsActive = true,
                EmailConfirmed = false,
                IsProfileCreated = true,
                IsProfileCompleted = false,
                IsSocialLogin = false
            };
            newUser.CreatedBy = newUser.Id;
            newUser.ModifiedBy = newUser.Id;
            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
                throw new BadRequestException($"{Constants.ErrorCreatingUser}: {result.Errors.First().Description}");

            _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());
            
            await SendEmailConfirmationCodeAsync(newUser);
            await CreateProfileAsync(newUser, signupCommand);
            otp = newUser.EmailConfirmationCode;
        }

        var message=LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
        return $"{message}: {otp}";
    }

    public async Task<Tuple<string, LoginDto>> UpdateUserPasswordAsync(string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        PasswordHasher<ApplicationUser> passwordHasher = new();

        existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);

        var result = await _userManager.UpdateAsync(existingUser);

        if (!result.Succeeded)
            throw new BadRequestException($"Error while updating user password {result.Errors.First().Description}");
        string token = await GenerateTokenAsync(existingUser);
        //return new Tuple<Guid, string>(existingUser.Id, Constants.PasswordUpdated);
        var loginDto = new LoginDto(token)
        {
            Email = existingUser.Email,
            UserId = existingUser.Id,
            AppleId = existingUser.AppleId,
            IsProfileCreated = existingUser.IsProfileCreated,
            IsProfileCompleted = existingUser.IsProfileCompleted,
        };
        return new Tuple<string, LoginDto>(Constants.PasswordUpdated, loginDto);
    }

    public async Task<string> EmailConfirmationAsync(string email, string code)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        if (existingUser.EmailConfirmationCode != code)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidOtp));

        if (existingUser.EmailConfirmationCodeExpiry < DateTime.UtcNow)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpExpired));

        existingUser.EmailConfirmed = true;
        existingUser.EmailConfirmationCode = null;
        existingUser.EmailConfirmationCodeExpiry = null;
        var result = await _userManager.UpdateAsync(existingUser);

        if (!result.Succeeded)
            throw new BadRequestException($"Error while confirming user email {result.Errors.First().Description}");

        var message=LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
        return $"{message}: {existingUser.EmailConfirmationCode}";
    }
    public async Task<string> ResendEmailConfirmationAsync(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        await SendEmailConfirmationCodeAsync(existingUser);

        var message=LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
        return $"{message}: {existingUser.EmailConfirmationCode}";
    }
    public async Task<string> ResendResetPasswordCodeAsync(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null || existingUser.IsDeleted)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        if (!existingUser.IsActive)
            throw new UnAuthorizedException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserAccountDisabled));

        await SendPasswordResetEmailAsync(existingUser);

        var message=LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully);
        return $"{message}: {existingUser.EmailConfirmationCode}";
    }



    private async Task CreateProfileAsync(ApplicationUser existingUser, SignupCommand? signupCommand = null)
    {
        var profileUrl = string.Concat(_clientUrlSettings.ProfileUrl, "/api/profiles/create-profile");

        var profileObj = new
        {
            existingUser.Id,
            signupCommand?.FirstName,
            signupCommand?.LastName,
            existingUser.Email,
            signupCommand?.DateOfBirth,
            signupCommand?.IsReceiveBlackRiseEmails,
            signupCommand?.IsReconnectWithEmail,
        };

        await _httpWrapper.PostAsync<object, object>(profileUrl, profileObj);
    }

    private async Task UpdateProfileAsync(ApplicationUser existingUser, SignupCommand signupCommand)
    {
        var profileUrl = string.Concat(_clientUrlSettings.ProfileUrl, "/api/profiles/update-profile");

        var profileObj = new
        {
            existingUser.Id,
            signupCommand.FirstName,
            signupCommand.LastName,
            existingUser.Email,
            signupCommand.DateOfBirth,
            signupCommand.IsReceiveBlackRiseEmails,
            signupCommand.IsReconnectWithEmail,
        };

        //await _httpWrapper.PostAsync<object, object>(profileUrl, profileObj);
    }

    public async Task<string> ForgotPasswordAsync(string email)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null || existingUser.IsDeleted)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        if (!existingUser.IsActive)
            throw new UnAuthorizedException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UserAccountDisabled));

        if(existingUser.IsSocialLogin)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.PasswordResetNotAvailableForSocialLogin));

        if (!existingUser.IsSocialLogin && existingUser.PasswordHash == null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.AccountDoesNotExist));

        await SendPasswordResetEmailAsync(existingUser);

        return $"{LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpSentSuccessfully)}: {existingUser.ResetPasswordCode}";
    }

    public async Task<string> ResetConfirmationAsync(string email, string code)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        if (existingUser.ResetPasswordCode != code)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidOtp));

        if (existingUser.ResetPasswordCodeExpiry < DateTime.UtcNow)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OtpExpired));

        existingUser.ResetPasswordCode = null;
        existingUser.ResetPasswordCodeExpiry = null;
        var result = await _userManager.UpdateAsync(existingUser);

        if (!result.Succeeded)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.ErrorCreatingUser));

        return LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailValidated);
    }


    public async Task<string> ResetPasswordAsync(string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null || existingUser.IsDeleted)
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InvalidUserEmail));

        var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

        // Ensure new password is not the same as the old one
        var passwordHasher = _userManager.PasswordHasher;
        if (passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password) == PasswordVerificationResult.Success)
        {
            throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.DifferentNewPassword));
        }

        var resetPasswordResult = await _userManager.ResetPasswordAsync(existingUser, token, password);

        if (!resetPasswordResult.Succeeded)
            throw new BadRequestException(resetPasswordResult.Errors.First().Description);

        return LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.Success);
    }

    private async Task<string> GenerateTokenAsync(ApplicationUser user)
    {
        IList<Claim> userClaims = await _userManager.GetClaimsAsync(user);
        IList<string> roles = await _userManager.GetRolesAsync(user);

        List<Claim> roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, roles?[0].ToString()),
        };

        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        var userRole = await _roleManager.FindByNameAsync(roles[0]);

        if(userRole != null)
        {
            var roleClaim = await _roleManager.GetClaimsAsync(userRole);
            claims.AddRange(roleClaim);
        }

        return GenerateToken(claims);
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signinCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expireTime = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expireTime,
            signingCredentials: signinCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    private async Task<BaseResponseDto<string>> SendEmailConfirmationCodeAsync(ApplicationUser applicationUser)
    {
        var code = await GenerateEmailConfirmationCodeAsync(applicationUser);

        var messageBody = $"<p> Hi </p> <br /><br /> <p> Your confirmation code is {code}. It will expire in 10 minutes.";

        var senderUrl = string.Concat(_clientUrlSettings.SenderUrl, "/api/email-sender/send-email");

        var reqBody = new
        {
            email = applicationUser.Email,
            subject = "Email Confirmation Code",
            body = messageBody
        };

        var result = await _httpWrapper.PostAsync<object, BaseResponseDto<string>>(senderUrl, reqBody);

        return result;
    }

    public async Task<string> GenerateEmailConfirmationCodeAsync(ApplicationUser user)
    {
        var code = new Random().Next(100000, 999999).ToString();
        user.EmailConfirmationCode = code;
        user.EmailConfirmationCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);
        return code;
    }

    public async Task<string> GeneratePasswordResetCodeAsync(ApplicationUser user)
    {
        var code = new Random().Next(100000, 999999).ToString();
        user.ResetPasswordCode = code;
        user.ResetPasswordCodeExpiry = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);
        return code;
    }

    private async Task<BaseResponseDto<string>> SendPasswordResetEmailAsync(ApplicationUser applicationUser)
    {
        var code = await GeneratePasswordResetCodeAsync(applicationUser);
        var senderUrl = string.Concat(_clientUrlSettings.SenderUrl, "/api/email-sender/send-email");
        var messageBody = $"<p> Hi </p> <br /><br /> <p>below is the password reset code. <br /><br /> {code} <br /><br /> <p>Thanks</p> </p>";

        var reqBody = new
        {
            email = applicationUser.Email,
            subject = "Password Reset Code",
            body = messageBody
        };

        var result = await _httpWrapper.PostAsync<object, BaseResponseDto<string>>(senderUrl, reqBody);

        return result;
    }
    public async Task<LoginDto> LoginWithGoogleAsync(string accessToken)
    { 
        using var client = new HttpClient();

        var tokenValidationResponse = await client.GetAsync($"{_googleSetting.GoogleOauthUrl}{accessToken}");
        tokenValidationResponse.EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var userInfoResponse = await client.GetAsync(_googleSetting.GoogleUserInfoUrl);
        userInfoResponse.EnsureSuccessStatusCode();

        var json = await userInfoResponse.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
            throw new UnauthorizedAccessException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.GoogleLoginNotVerified));

        dynamic userInfo = JsonConvert.DeserializeObject(json);
        string? email = userInfo?.email;
        string? firstName = userInfo?.given_name;
        string? lastName = userInfo?.family_name;

        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedAccessException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.GoogleLoginNotVerified));

        return await HandleExternalLoginAsync(email, firstName, lastName);
    }


    public async Task<LoginDto> LoginWithLinkedInAsync(string code)
    {
        var gettoken = await GetLinkedInToken(code);
        var linkedInUser = await GetLinkedInUserProfileAsync(gettoken);

        if (linkedInUser == null || string.IsNullOrEmpty(linkedInUser.Email))
            throw new UnauthorizedAccessException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.LinkedInLoginNotVerified));

        return await HandleExternalLoginAsync(linkedInUser.Email, linkedInUser.FirstName, linkedInUser.LastName);
    }

    public async Task<string> GetLinkedInToken(string authorizationCode)
    {
        var client = new HttpClient();

        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", authorizationCode },
            { "redirect_uri", _linkedInSetting.LinkedInRedirectUri },
            { "client_id", _linkedInSetting.LinkedInClientId },
            { "client_secret", _linkedInSetting.LinkedInClientSecret }
        };

        var content = new FormUrlEncodedContent(parameters);
        var response = await client.PostAsync(_linkedInSetting.LinkedInOauthUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error fetching access token: {error}");
        }

        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = responseJson.GetProperty("access_token").GetString();

        return accessToken;
    }
    private async Task<LinkedInUser> GetLinkedInUserProfileAsync(string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

        var response = await client.GetAsync(_linkedInSetting.LinkedInUserInfoUrl);
        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonDocument.Parse(content);

        var email = userInfo.RootElement.GetProperty("email").GetString();
        var firstName = userInfo.RootElement.GetProperty("given_name").GetString();
        var lastName = userInfo.RootElement.GetProperty("family_name").GetString();

        return new LinkedInUser
        {
            FirstName = firstName ?? "LinkedIn user",
            LastName = lastName ?? "",
            Email = email ?? ""
        };
    }
    public async Task<LoginDto> LoginWithAppleAsync(string accessToken)
    {
        _logger.LogInformation($"Verifying Apple ID token: {accessToken}");
        var appleJwt = await VerifyAppleIdTokenAsync(accessToken);
        if (appleJwt == null)
            throw new UnauthorizedAccessException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.AppleLoginNotVerified));

        var email = appleJwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var userId = appleJwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.AppleLoginNotVerified));

        email = email ?? $"{userId}@appleid.local";

        return await HandleExternalLoginAsync(email, null, null, "Apple", userId, true);
    }


    private async Task<LoginDto> HandleExternalLoginAsync(string email, string? firstName, string? lastName, string? provider = null, string? providerUserId = null, bool isApple = false)
    {
        ApplicationUser? user = null;

        if (!string.IsNullOrEmpty(provider) && !string.IsNullOrEmpty(providerUserId))
            user = await _userManager.FindByLoginAsync(provider, providerUserId);

        if (user == null && email != null)
            user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email ?? providerUserId,
                Email = email ?? "",
                NormalizedEmail = (email ?? "").ToUpper(),
                NormalizedUserName = (email ?? "").ToUpper(),
                EmailConfirmed = true,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                AppleId = isApple ? providerUserId : null,
                IsProfileCreated = false,
                IsProfileCompleted = false,
                IsSocialLogin = true,
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new ArgumentException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.CouldNotCreateAccount));

            if (isApple)
            {
                var loginInfo = new UserLoginInfo(provider, providerUserId, provider);
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!loginResult.Succeeded)
                    throw new ArgumentException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.LoginProviderNotAdded));
            }

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
        }
        else
        {
            if(!user.IsSocialLogin)
                throw new BadRequestException(LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.EmailAlreadyRegistered));
        }

        var token = await GenerateTokenAsync(user);
        var loginDto = new LoginDto(token)
        {
            Email = user.Email,
            UserId = user.Id,
            AppleId = user.AppleId,
            FirstName = firstName,
            LastName = lastName,
            IsProfileCreated = user.IsProfileCreated,
            IsProfileCompleted = user.IsProfileCompleted,
        };
        return loginDto;
    }
    public async Task<JwtSecurityToken> VerifyAppleIdTokenAsync(string accessToken)
    {
        var keys = await GetAppleKeysAsync();

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        var kid = jwt.Header.Kid;
        var alg = jwt.Header.Alg;

        var key = keys?.First(k => k?["kid"]?.ToString() == kid && k?["alg"]?.ToString() == alg);

        var rsa =  CreateSecurityKeyFromJwk(key, kid);

        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = rsa,
            ValidIssuer = _appleSetting.Issuer,
            ValidAudience = _appleSetting.Audience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        try
        {
            handler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            return (JwtSecurityToken)validatedToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }

    private async Task<JArray?> GetAppleKeysAsync()
    {
        using var client = new HttpClient();
        var keysJson = await client.GetStringAsync(_appleSetting.AuthKeysUrl);
        var keys = JObject.Parse(keysJson)?["keys"] as JArray;
        return keys;
    }
    private static RsaSecurityKey CreateSecurityKeyFromJwk(JToken key, string kid)
    {
        var e = Base64UrlEncoder.DecodeBytes(key["e"]!.ToString());
        var n = Base64UrlEncoder.DecodeBytes(key["n"]!.ToString());

        var rsaParameters = new RSAParameters { Exponent = e, Modulus = n };
        var rsa = RSA.Create();
        rsa.ImportParameters(rsaParameters);

        return new RsaSecurityKey(rsa) { KeyId = kid };
    }
}
