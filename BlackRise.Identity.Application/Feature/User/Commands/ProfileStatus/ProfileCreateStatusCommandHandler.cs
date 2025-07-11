using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using MediatR;

namespace BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus
{
    public class ProfileCreateStatusCommandHandler : IRequestHandler<ProfileCreateStatusCommand, BaseResponseDto<string>>
    {
        private readonly IUserService _userService;
        public ProfileCreateStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponseDto<string>> Handle(ProfileCreateStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUserProfileCreateStatus(request);
            return new BaseResponseDto<string>(result);
        }
    }
}
