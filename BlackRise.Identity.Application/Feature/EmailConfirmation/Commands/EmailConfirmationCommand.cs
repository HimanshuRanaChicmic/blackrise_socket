using MediatR;

namespace BlackRise.Identity.Application.Feature.EmailConfirmation.Commands;

public class EmailConfirmationCommand : IRequest<EmailConfirmationDto>
{
    public string Email { get; set; }
    public string Token { get; set; }
}
