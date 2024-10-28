using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
{
    private readonly IAuthService _authService;
    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginResult = await _authService.LoginAsync(request.Email, request.Password);
        return new LoginDto(loginResult);
    }
}
