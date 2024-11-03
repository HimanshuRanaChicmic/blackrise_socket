using MediatR;

namespace BlackRise.Identity.Application.Feature.VerifyResetPasswordCode.Commands;

public class VerifyResetPasswordCodeCommand : IRequest<VerifyResetPasswordCodeDto>
{
    public string Email { get; set; }
    public string Code { get; set; }
}
