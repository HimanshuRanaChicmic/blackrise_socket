using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.ResendResetPassword.Commands;

public class ResendResetPasswordCommandHandler : IRequestHandler<ResendResetPasswordCommand, ResendResetPasswordDto>
{
    private readonly IAuthService _authService;
    public ResendResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ResendResetPasswordDto> Handle(ResendResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var emailConfirmationResult = await _authService.ResendResetPasswordCodeAsync(request.Email);

        return new ResendResetPasswordDto(emailConfirmationResult);
    }
}
