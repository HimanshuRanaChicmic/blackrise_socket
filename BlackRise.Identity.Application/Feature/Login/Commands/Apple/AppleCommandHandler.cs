using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.Apple
{
    public class AppleCommandHandler : IRequestHandler<AppleCommand, LoginDto>
    {
        private readonly IAuthService _authService;
        public AppleCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginDto> Handle(AppleCommand request, CancellationToken cancellationToken)
        {
            var loginResult = await _authService.LoginWithAppleAsync(request.AccessToken);
            return new LoginDto(loginResult);
        }
    }
}
