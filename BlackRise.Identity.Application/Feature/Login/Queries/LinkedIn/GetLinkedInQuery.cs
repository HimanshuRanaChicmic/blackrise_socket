using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Queries.LinkedIn;

public class GetLinkedInQuery : IRequest<string>
{
    public string Code { get; set; }
}
