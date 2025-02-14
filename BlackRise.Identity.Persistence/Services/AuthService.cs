using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Exceptions;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Domain;
using BlackRise.Identity.Domain.Common.Enums;
using BlackRise.Identity.Persistence.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlackRise.Identity.Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JwtSetting _jwtSettings;
    private readonly ClientUrlSetting _clientUrlSettings;
    private readonly IHttpWrapper _httpWrapper;
    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSetting> jwtSettings,
        IOptions<ClientUrlSetting> clientUrlSettings, IHttpWrapper httpWrapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _clientUrlSettings = clientUrlSettings.Value;
        _httpWrapper = httpWrapper;
        _jwtSettings = jwtSettings.Value;

    }

    public async Task<string> LoginAsync(string username, string password)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null || user.IsDeleted)
                throw new BadRequestException("Invalid email/password");

            if(!user.IsActive)
                throw new BadRequestException("User account disabled");

            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

            if (result.IsNotAllowed)
                throw new BadRequestException($"email not confirm");

            if (!result.Succeeded)
                throw new BadRequestException($"invalid username/password");

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
                throw new BadRequestException("User already exists");

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
                throw new BadRequestException($"Error while creating user {result.Errors.First().Description}");

            _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());

            await SendEmailConfirmationCodeAsync(newUser);

            return "Success";
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

            if (existingUser != null)
                throw new BadRequestException("User already exists");

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

            var result = await _userManager.CreateAsync(newUser, "Temp@13221212");

            if (!result.Succeeded)
                throw new BadRequestException($"Error while creating user {result.Errors.First().Description}");

            _ = await _userManager.AddToRoleAsync(newUser, Role.User.ToString());

            await SendEmailConfirmationCodeAsync(newUser);
            await CreateProfileAsync(newUser, signupCommand);

            return "Success";
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
                throw new BadRequestException("Invalid user");

            PasswordHasher<ApplicationUser> passwordHasher = new();

            existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, password);

            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while updating user password {result.Errors.First().Description}");

            return new Tuple<Guid, string>(existingUser.Id,"Success");
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
                throw new BadRequestException("Invalid user email");

            if (existingUser.EmailConfirmationCode != code)
                throw new BadRequestException("Invalid code");

            if (existingUser.EmailConfirmationCodeExpiry < DateTime.UtcNow)
                throw new BadRequestException("Code has expired");

            existingUser.EmailConfirmed = true;
            existingUser.EmailConfirmationCode = null;
            existingUser.EmailConfirmationCodeExpiry = null;
            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirm user email {result.Errors.First().Description}");

            return "Success";
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task CreateProfileAsync(ApplicationUser existingUser, SignupCommand signupCommand)
    {
        var profileUrl = string.Concat(_clientUrlSettings.ProfileUrl, "/api/profiles/create-profile");

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

        await _httpWrapper.PostAsync<object, object>(profileUrl, profileObj);
    }

    public async Task<string> ForgotPasswordAsync(string email)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser == null || existingUser.IsDeleted)
                throw new BadRequestException("Invalid user email");

            if (!existingUser.IsActive)
                throw new UnAuthorizedException($"user account disabled");

            await SendPasswordResetEmailAsync(existingUser);

            return "Success";
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
                throw new BadRequestException("Invalid user email");

            if (existingUser.ResetPasswordCode != code)
                throw new BadRequestException("Invalid code");

            if (existingUser.ResetPasswordCodeExpiry < DateTime.UtcNow)
                throw new BadRequestException("Code has expired");

            existingUser.ResetPasswordCode = null;
            existingUser.ResetPasswordCodeExpiry = null;
            var result = await _userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirm user email {result.Errors.First().Description}");

            return "Success";
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
                throw new BadRequestException("Invalid user email");

            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

            var resetPasswordResult = await _userManager.ResetPasswordAsync(existingUser, token, password);

            if (!resetPasswordResult.Succeeded)
              throw new BadRequestException(resetPasswordResult.Errors.First().Description);

            return "Success";
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
            new(ClaimTypes.Role, roles[0].ToString()),
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

            var messageBody = $"<p> Hi </p> <br /><br /> <p> Your confirmation code is {code}. It will expire in 2 minutes.";

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
        user.EmailConfirmationCodeExpiry = DateTime.UtcNow.AddMinutes(2);
        await _userManager.UpdateAsync(user);
        return code;
    }

    public async Task<string> GeneratePasswordResetCodeAsync(ApplicationUser user)
    {
        var code = new Random().Next(100000, 999999).ToString();
        user.ResetPasswordCode = code;
        user.ResetPasswordCodeExpiry = DateTime.UtcNow.AddMinutes(2);
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
}
