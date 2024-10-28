namespace BlackRise.Identity.Application.Feature.Login;

public class LoginDto(string token)
{
    public string Token { get; set; } = token;
}
