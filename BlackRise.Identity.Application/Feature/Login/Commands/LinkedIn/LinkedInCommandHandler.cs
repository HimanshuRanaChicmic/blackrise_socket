using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.LinkedIn;

public class LinkedInCommandHandler : IRequestHandler<LinkedInCommand, LoginDto>
{
    private readonly IAuthService _authService;
    public LinkedInCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginDto> Handle(LinkedInCommand request, CancellationToken cancellationToken)
    {
        var loginResult = await _authService.LoginWithLinkedInAsync(request.AccessToken);
        return new LoginDto(loginResult);
    }
}
