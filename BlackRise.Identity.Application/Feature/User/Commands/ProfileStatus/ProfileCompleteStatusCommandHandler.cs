using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.DataTransferObject;
using MediatR;

namespace BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus
{
    public class ProfileCompleteStatusCommandHandler : IRequestHandler<ProfileCompleteStatusCommand, BaseResponseDto<string>>
    {
        private readonly IUserService _userService;
        public ProfileCompleteStatusCommandHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<BaseResponseDto<string>> Handle(ProfileCompleteStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUserProfileCompleteStatus(request);
            return new BaseResponseDto<string>(result);
        }
    }
}
