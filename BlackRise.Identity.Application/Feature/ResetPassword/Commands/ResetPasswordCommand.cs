using MediatR;

namespace BlackRise.Identity.Application.Feature.ResetPassword.Commands;

public class ResetPasswordCommand : IRequest<ResetPasswordDto>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
}
