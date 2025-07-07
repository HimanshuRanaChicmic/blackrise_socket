using MediatR;

namespace BlackRise.Identity.Application.Feature.Login.Commands.Apple
{
    public class AppleCommand : IRequest<LoginDto>
    {
        public string AccessToken { get; set; }
    }
}
