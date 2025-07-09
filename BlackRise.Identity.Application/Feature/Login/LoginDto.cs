namespace BlackRise.Identity.Application.Feature.Login;

public class LoginDto(string token)
{
    public string Token { get; set; } = token;
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? AppleId { get; set; }
}
