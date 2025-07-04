using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.Google
{
    public class GoogleCommandHandler : IRequestHandler<GoogleCommand, LoginDto>
    {
        private readonly IAuthService _authService;
        public GoogleCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginDto> Handle(GoogleCommand request, CancellationToken cancellationToken)
        {
            var loginResult = await _authService.LoginWithGoogleAsync(request.AccessToken);
            return new LoginDto(loginResult);
        }
    }
}
