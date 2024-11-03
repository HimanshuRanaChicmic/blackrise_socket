using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.VerifyResetPasswordCode.Commands;

public class VerifyResetPasswordCodeCommandHandler : IRequestHandler<VerifyResetPasswordCodeCommand, VerifyResetPasswordCodeDto>
{
    private readonly IAuthService _authService;
    public VerifyResetPasswordCodeCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<VerifyResetPasswordCodeDto> Handle(VerifyResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        var resetTokenResult = await _authService.ResetConfirmationAsync(request.Email, request.Code);

        return new VerifyResetPasswordCodeDto(resetTokenResult);
    }
}
