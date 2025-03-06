using MediatR;

namespace BlackRise.Identity.Application.Feature.ResendResetPassword.Commands;

public class ResendResetPasswordCommand : IRequest<ResendResetPasswordDto>
{
    public string Email { get; set; }
}
