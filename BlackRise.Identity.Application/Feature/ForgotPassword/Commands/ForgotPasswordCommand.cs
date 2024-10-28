using MediatR;

namespace BlackRise.Identity.Application.Feature.ForgotPassword.Commands;

public class ForgotPasswordCommand : IRequest<ForgotPasswordDto>
{
    public string Email { get; set; }
}
