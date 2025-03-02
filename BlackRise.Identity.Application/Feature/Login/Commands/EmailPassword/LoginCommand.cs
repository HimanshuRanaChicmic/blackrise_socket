using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.EmailPassword;

public class LoginCommand : IRequest<LoginDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
