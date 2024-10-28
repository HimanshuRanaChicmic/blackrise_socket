namespace BlackRise.Identity.Application.DataTransferObject;

public class BaseResponseDto(string result)
{
    public string Result { get; set; } = result;
}
