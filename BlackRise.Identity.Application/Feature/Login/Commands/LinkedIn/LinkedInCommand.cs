using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.LinkedIn;

public class LinkedInCommand : IRequest<LoginDto>
{
    public string AccessToken { get; set; }
}
