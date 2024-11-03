using BlackRise.Identity.Application.Contracts;
using MediatR;

namespace BlackRise.Identity.Application.Feature.EmailConfirmation.Commands;

public class EmailConfirmationCommandHandler : IRequestHandler<EmailConfirmationCommand, EmailConfirmationDto>
{
    private readonly IAuthService _authService;
    public EmailConfirmationCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<EmailConfirmationDto> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var emailConfirmationResult = await _authService.EmailConfirmationAsync(request.Email, request.Code);

        return new EmailConfirmationDto(emailConfirmationResult);
    }
}
