using BlackRise.Identity.Application.Feature.Login;
using BlackRise.Identity.Application.Feature.Signup.Commands;
using BlackRise.Identity.Application.Feature.User;

namespace BlackRise.Identity.Application.Contracts;

public interface IAuthService
{
    Task<string> RegisterAsync(string username, string password);
    Task<string> RegisterAsync(SignupCommand signupCommand);
    Task<string> EmailConfirmationAsync(string email, string code);
    Task<string> ResendEmailConfirmationAsync(string email);
    Task<string> ResendResetPasswordCodeAsync(string email);
    Task<LoginDto> LoginAsync(string username, string password);
    Task<LoginDto> LoginWithLinkedInAsync(string accessToken);
    Task<LoginDto> LoginWithGoogleAsync(string accessToken);
    Task<LoginDto> LoginWithAppleAsync(string accessToken);
    Task<string> ForgotPasswordAsync(string email);
    Task<string> ResetConfirmationAsync(string email, string code);
    Task<string> ResetPasswordAsync(string email, string password);

    Task<Tuple<string, LoginDto>> UpdateUserPasswordAsync(string email, string password);
}
