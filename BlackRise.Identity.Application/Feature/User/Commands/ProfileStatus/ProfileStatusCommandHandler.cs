using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using MediatR;

namespace BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus
{
    public class ProfileStatusCommandHandler : IRequestHandler<ProfileStatusCommand, BaseResponseDto<string>>
    {
        private readonly IUserService _userService;
        public ProfileStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<BaseResponseDto<string>> Handle(ProfileStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUserProfileStatus(request);
            return new BaseResponseDto<string>(result);
        }
    }
}
