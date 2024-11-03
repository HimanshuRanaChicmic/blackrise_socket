using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.VerifyResetPasswordCode;

public class VerifyResetPasswordCodeDto(string result) : BaseResponseDto(result)
{
}
