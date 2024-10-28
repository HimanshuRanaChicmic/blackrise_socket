using FluentValidation.Results;

namespace BlackRise.Identity.Application.Exceptions;

public class ValidationException : ApplicationException
{
    public List<string> ValidationErrors { get; set; }

    public ValidationException(ValidationResult validationResult)
    {
        ValidationErrors = new List<string>();

        foreach (ValidationFailure? validationError in validationResult.Errors)
        {
            ValidationErrors.Add(validationError.ErrorMessage);
        }
    }
}
