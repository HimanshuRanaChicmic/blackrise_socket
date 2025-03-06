using MediatR;

namespace BlackRise.Identity.Application.Feature.ResendEmailConfirmation.Commands;

public class ResendEmailConfirmationCommand : IRequest<ResendEmailConfirmationDto>
{
    public string Email { get; set; }
}
