using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.ForgotPassword;

public class ForgotPasswordDto(string result):BaseResponseDto<string>(result)
{
}
