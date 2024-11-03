using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.ResetPassword.Commands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordDto>
{
    private readonly IAuthService _authService;
    public ResetPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ResetPasswordDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetTokenResult =  await _authService.ResetPasswordAsync(request.Email, request.Password);

        return new ResetPasswordDto(resetTokenResult);
    }
}
