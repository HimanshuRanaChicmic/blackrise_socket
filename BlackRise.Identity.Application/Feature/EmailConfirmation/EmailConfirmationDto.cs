using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.EmailConfirmation;

public class EmailConfirmationDto(string result):BaseResponseDto<string>(result)
{
}
