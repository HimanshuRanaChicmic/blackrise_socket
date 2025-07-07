using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.Google
{
    public class GoogleCommand : IRequest<LoginDto>
    {
        public string AccessToken { get; set; }
    }
}
