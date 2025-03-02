using BlackRise.Identity.Application.Feature.Signup.Commands;

namespace BlackRise.Identity.Application.Contracts;

public interface IAuthService
{
    Task<string> RegisterAsync(string username, string password);
    Task<string> RegisterAsync(SignupCommand signupCommand);
    Task<string> EmailConfirmationAsync(string email, string code);
    Task<string> LoginAsync(string username, string password);
    Task<string> LoginWithLinkedInAsync(string linkedInCode);
    Task<string> ForgotPasswordAsync(string email);
    Task<string> ResetConfirmationAsync(string email, string code);
    Task<string> ResetPasswordAsync(string email, string password);

    Task<Tuple<Guid, string>> UpdateUserPasswordAsync(string email, string password);
}
