using BlackRise.Identity.Application.DataTransferObject;

namespace BlackRise.Identity.Application.Feature.Signup;

public class SignupDto(string result):BaseResponseDto<string>(result)
{
}
