using BlackRise.Identity.Application.DataTransferObject;
using MediatR;

namespace BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus
{
    public class ProfileCompleteStatusCommand : IRequest<BaseResponseDto<string>>
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public bool? IsProfileCreated { get; set; }
        public bool? IsProfileCompleted { get; set; }
    }
}
