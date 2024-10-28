using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.ForgotPassword.Commands;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordDto>
{
    private readonly IAuthService _authService;
    public ForgotPasswordCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ForgotPasswordDto> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var forgotResult = await _authService.ForgotPasswordAsync(request.Email);

        return new ForgotPasswordDto(forgotResult);
    }
}
