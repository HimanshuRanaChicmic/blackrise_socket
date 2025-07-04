using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Exceptions;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Application.Settings;
using BlackRise.Identity.Domain;
using BlackRise.Identity.Domain.Common.Enums;
using BlackRise.Identity.Persistence.Settings;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BlackRise.Identity.Persistence.Utils;

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
    private readonly IHttpWrapper _httpWrapper;
    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSetting> jwtSettings,
        IOptions<AppleSetting> appleSetting,
        IOptions<ClientUrlSetting> clientUrlSettings, IHttpWrapper httpWrapper, IOptions<LinkedInSetting> linkedInSetting)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _clientUrlSettings = clientUrlSettings.Value;
        _httpWrapper = httpWrapper;
        _jwtSettings = jwtSettings.Value;
        _linkedInSetting = linkedInSetting.Value;
        _appleSetting = appleSetting.Value;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null || user.IsDeleted)
                throw new BadRequestException(Constants.InvalidEmailPassword);

            if(user.PasswordHash == null)
                throw new BadRequestException(Constants.UserPasswordNotSet);

            if (!user.IsActive)
                throw new BadRequestException(Constants.UserAccountDisabled);

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.IsNotAllowed)
                throw new BadRequestException(Constants.EmailNotConfirmed);

            if (!result.Succeeded)
                throw new BadRequestException(Constants.InvalidUsernamePassword);

            string token = await GenerateTokenAsync(user);

            return token;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> RegisterAsync(string username, string password)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(username);

            if (existingUser != null)
                throw new BadRequestException(Constants.EmailAlreadyRegistered);

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
                throw new BadRequestException($"{Constants.ErrorCreatingUser}: {result.Errors.First().Description}");

            _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());

            await SendEmailConfirmationCodeAsync(newUser);

            return Constants.OtpSentSuccessfully;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> RegisterAsync(SignupCommand signupCommand)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(signupCommand.Email);

            if (existingUser != null && existingUser.EmailConfirmed && existingUser.PasswordHash != null)
                throw new BadRequestException(Constants.EmailAlreadyRegistered);

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
            };
            newUser.CreatedBy = newUser.Id;
            newUser.ModifiedBy = newUser.Id;

           
            if (existingUser != null && !existingUser.EmailConfirmed)
            {
                newUser.Id = existingUser.Id;
                await UpdateProfileAsync(newUser, signupCommand);

                await SendEmailConfirmationCodeAsync(existingUser);

            }
            else
            {
                var result = await _userManager.CreateAsync(newUser);

                if (!result.Succeeded)
                    throw new BadRequestException($"{Constants.ErrorCreatingUser}: {result.Errors.First().Description}");

                _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());
                await CreateProfileAsync(newUser, signupCommand);

                await SendEmailConfirmationCodeAsync(newUser);
            }


            return Constants.OtpSentSuccessfully;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<Tuple<Guid, string>> UpdateUserPasswordAsync(string email, string password)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
                throw new BadRequestException(Constants.InvalidUserEmail);

            PasswordHasher<ApplicationUser> passwordHasher = new();

            existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while updating user password {result.Errors.First().Description}");

            return new Tuple<Guid, string>(existingUser.Id, Constants.PasswordUpdated);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> EmailConfirmationAsync(string email, string code)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
                throw new BadRequestException(Constants.InvalidUserEmail);

            if (existingUser.EmailConfirmationCode != code)
                throw new BadRequestException(Constants.InvalidOtp);

            if (existingUser.EmailConfirmationCodeExpiry < DateTime.UtcNow)
                throw new BadRequestException(Constants.OtpExpired);

            existingUser.EmailConfirmed = true;
            existingUser.EmailConfirmationCode = null;
            existingUser.EmailConfirmationCodeExpiry = null;
            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirming user email {result.Errors.First().Description}");

            return Constants.OtpVerifiedSuccessfully;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<string> ResendEmailConfirmationAsync(string email)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
                throw new BadRequestException(Constants.InvalidUserEmail);

            await SendEmailConfirmationCodeAsync(existingUser);

            return Constants.OtpSentSuccessfully;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task<string> ResendResetPasswordCodeAsync(string email)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null || existingUser.IsDeleted)
                throw new BadRequestException(Constants.InvalidUserEmail);

            if (!existingUser.IsActive)
                throw new UnAuthorizedException(Constants.UserAccountDisabled);

            await SendPasswordResetEmailAsync(existingUser);

            return Constants.OtpSentSuccessfully;
        }
        catch (Exception ex)
        {
            throw;
        }
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
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null || existingUser.IsDeleted)
                throw new BadRequestException(Constants.InvalidUserEmail);

            if (!existingUser.IsActive)
                throw new UnAuthorizedException(Constants.UserAccountDisabled);

            if(existingUser.PasswordHash == null)
                throw new BadRequestException(Constants.AccountDoesNotExist);

            await SendPasswordResetEmailAsync(existingUser);

            return "OTP sent successfully.";
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> ResetConfirmationAsync(string email, string code)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null)
                throw new BadRequestException(Constants.InvalidUserEmail);

            if (existingUser.ResetPasswordCode != code)
                throw new BadRequestException(Constants.InvalidOtp);

            if (existingUser.ResetPasswordCodeExpiry < DateTime.UtcNow)
                throw new BadRequestException(Constants.OtpExpired);

            existingUser.ResetPasswordCode = null;
            existingUser.ResetPasswordCodeExpiry = null;
            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirm user email {result.Errors.First().Description}");

            return Constants.Success;
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    public async Task<string> ResetPasswordAsync(string email, string password)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null || existingUser.IsDeleted)
                throw new BadRequestException(Constants.InvalidUserEmail);

            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            // Ensure new password is not the same as the old one
            var passwordHasher = _userManager.PasswordHasher;
            if (passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, password) == PasswordVerificationResult.Success)
            {
                throw new BadRequestException(Constants.DifferentNewPassword);
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(existingUser, token, password);

            if (!resetPasswordResult.Succeeded)
              throw new BadRequestException(resetPasswordResult.Errors.First().Description);

            return Constants.Success;
        }
        catch (Exception)
        {
            throw;
        }
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

    private async Task<BaseResponseDto> SendEmailConfirmationCodeAsync(ApplicationUser applicationUser)
    {
        try
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

            var result = await _httpWrapper.PostAsync<object, BaseResponseDto>(senderUrl, reqBody);

            return result;
        }
        catch (Exception ex)
        {
            throw;
        }

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

    private async Task<BaseResponseDto> SendPasswordResetEmailAsync(ApplicationUser applicationUser)
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

        var result = await _httpWrapper.PostAsync<object, BaseResponseDto>(senderUrl, reqBody);

        return result;
    }

    public async Task<string> LoginWithGoogleAsync(string accessToken)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException(Constants.GoogleLoginNotVerified, ex);
        }
        catch (Exception ex)
        {
            throw new Exception(Constants.GoogleLoginNotVerified, ex);
        }
        if (payload == null)
            throw new UnauthorizedAccessException(Constants.GoogleLoginNotVerified);

        var email = payload.Email;
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(Constants.AccountNotCreatedWithGoogle);

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
        }

        string token = await GenerateTokenAsync(user);
        return token;
    }
    public async Task<string> LoginWithLinkedInAsync(string accessToken)
    {
        LinkedInUser linkedInUser;
        try
        {
            linkedInUser = await GetLinkedInUserProfileAsync(accessToken);

            if (linkedInUser == null || string.IsNullOrEmpty(linkedInUser.Email))
                throw new UnauthorizedAccessException(Constants.LinkedInLoginNotVerified);
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException(Constants.LinkedInLoginNotVerified, ex);
        }
        catch (Exception ex)
        {
            throw new Exception(Constants.LinkedInLoginNotVerified, ex);
        }
        if (linkedInUser == null)
            throw new UnauthorizedAccessException(Constants.LinkedInLoginNotVerified);


        var user = await _userManager.FindByEmailAsync(linkedInUser.Email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = linkedInUser.Email,
                Email = linkedInUser.Email,
                NormalizedEmail = linkedInUser.Email.ToUpper(),
                NormalizedUserName = linkedInUser.Email.ToUpper(),
                EmailConfirmed = true,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new Exception(Constants.AccountNotCreatedWithLinkedIn);

            await _userManager.AddToRoleAsync(user, Role.User.ToString());
        }

        var jwtToken = await GenerateTokenAsync(user);

        return jwtToken;
    }
    public async Task<string> LoginWithAppleAsync(string accessToken)
    {
        JwtSecurityToken appleJwt;
        try
        {
            appleJwt = await VerifyAppleIdTokenAsync(accessToken);
        }
        catch (InvalidJwtException ex)
        {
            throw new UnauthorizedAccessException(Constants.AppleLoginNotVerified, ex);
        }
        catch (Exception ex)
        {
            throw new Exception(Constants.AppleLoginNotVerified, ex);
        }
        if (appleJwt == null)
            throw new UnauthorizedAccessException(Constants.AppleLoginNotVerified);

        var email = appleJwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var emailVerified = appleJwt.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;
        var appleUserId = appleJwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(appleUserId))
            throw new UnauthorizedAccessException(Constants.AppleLoginNotVerified);

        var user = await _userManager.FindByLoginAsync("Apple", appleUserId);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email ?? appleUserId,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new Exception(Constants.AccountNotCreatedWithApple);

            await _userManager.AddLoginAsync(user, new UserLoginInfo("Apple", appleUserId, "Apple"));
            await CreateProfileAsync(user);
            await _userManager.AddToRoleAsync(user, Role.User.ToString());
        }

        var jwtToken = await GenerateTokenAsync(user);

        return jwtToken;
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
    private async Task<LinkedInUser> GetLinkedInUserProfileAsync(string accessToken)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync(_linkedInSetting.LinkedInUserProfileUrl);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to retrieve LinkedIn user profile");

        return await response.Content.ReadFromJsonAsync<LinkedInUser>();
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
