using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Feature.User;
using BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus;

namespace BlackRise.Identity.Application.Contracts
{
    public interface IUserService
    {
        Task<UserDto> GetUserById(Guid userId);
        Task<string> UpdateUserProfileStatus(ProfileStatusCommand request);
    }
}
