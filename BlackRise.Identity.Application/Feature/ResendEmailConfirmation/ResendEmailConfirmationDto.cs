using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.ResendEmailConfirmation;

public class ResendEmailConfirmationDto(string result):BaseResponseDto<string>(result)
{
}
