namespace BlackRise.Identity.Application.DataTransferObject;

public class ErrorDto
{
    public string Error { get; set; }

    public List<string> ValidationErrors { get; set; }

    public ErrorDto() { }

    public ErrorDto(List<string> ValidationErrors)
    {
        this.ValidationErrors = ValidationErrors;
    }

    public ErrorDto(string Error)
    {
        this.Error = Error;
    }
}
