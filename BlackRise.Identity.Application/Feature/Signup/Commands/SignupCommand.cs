using MediatR;

namespace BlackRise.Identity.Application.Feature.Signup.Commands;

public class SignupCommand : IRequest<SignupDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
}
