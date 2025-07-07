using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.ResendResetPassword;

public class ResendResetPasswordDto(string result):BaseResponseDto<string>(result)
{
}
