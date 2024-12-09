using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.Signup.Commands;

public  class SignupCommandHandler : IRequestHandler<SignupCommand, SignupDto>
{
    private readonly IAuthService _authService;
    public SignupCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<SignupDto> Handle(SignupCommand request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request);
        return new SignupDto(result);
    }
}
