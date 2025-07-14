using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Feature.Login;

namespace BlackRise.Identity.Application.Feature.UpdatePassword;

public class UpdatePasswordDto(string result, LoginDto loginData) : BaseResponseDto<string>(result)
{
    public LoginDto LoginData { get; set; } = loginData;
}
