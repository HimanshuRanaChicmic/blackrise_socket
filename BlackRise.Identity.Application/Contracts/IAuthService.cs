namespace BlackRise.Identity.Application.Contracts;

public interface IAuthService
{
    Task<string> RegisterAsync(string username, string password);
    Task<string> EmailConfirmationAsync(string email, string code);
    Task<string> LoginAsync(string username, string password);
    Task<string> ForgotPasswordAsync(string email);
    Task<string> ResetConfirmationAsync(string email, string code);
    Task<string> ResetPasswordAsync(string email, string password);
}
