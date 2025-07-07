using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.UpdatePassword;

public class UpdatePasswordDto(string result, Guid userId) : BaseResponseDto<string>(result)
{
    public Guid UserId { get; set; } = userId;
}
