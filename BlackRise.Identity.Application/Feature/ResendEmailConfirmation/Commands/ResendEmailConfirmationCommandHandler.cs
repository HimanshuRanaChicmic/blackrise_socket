using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.ResendEmailConfirmation.Commands;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, ResendEmailConfirmationDto>
{
    private readonly IAuthService _authService;
    public ResendEmailConfirmationCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<ResendEmailConfirmationDto> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var emailConfirmationResult = await _authService.ResendEmailConfirmationAsync(request.Email);

        return new ResendEmailConfirmationDto(emailConfirmationResult);
    }
}
