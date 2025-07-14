using BlackRise.Identity.Application.Contracts;
using BlackRise.Identity.Application.Exceptions;
using BlackRise.Identity.Application.Feature.User;
using BlackRise.Identity.Application.Feature.User.Commands.ProfileStatus;
using BlackRise.Identity.Domain;
using BlackRise.Identity.Persistence.Utils;
using Microsoft.AspNetCore.Identity;

namespace BlackRise.Identity.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException(Constants.UserNotFound);
            return new UserDto
            {
                Id = user.Id,
                AppleId = user.AppleId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsProfileCreated = user.IsProfileCreated,
                IsProfileCompleted = user.IsProfileCompleted
            };
        }

        public async Task<string> UpdateUserProfileCreateStatus(ProfileCreateStatusCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                throw new NotFoundException(Constants.UserNotFound);

            user.IsProfileCreated = request.IsProfileCreated == true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirm user email {result.Errors.First().Description}");

            return Constants.Success;
        }
        public async Task<string> UpdateUserProfileCompleteStatus(ProfileCompleteStatusCommand request)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                throw new NotFoundException(Constants.UserNotFound);

            user.IsProfileCompleted = request.IsProfileCompleted == true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException($"Error while confirm user email {result.Errors.First().Description}");

            return Constants.Success;
        }
    }
}
