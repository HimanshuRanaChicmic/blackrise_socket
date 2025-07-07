namespace BlackRise.Identity.Application.DataTransferObject;

public class BaseResponseDto<T>(T result)
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T Result { get; set; } = result;
}
