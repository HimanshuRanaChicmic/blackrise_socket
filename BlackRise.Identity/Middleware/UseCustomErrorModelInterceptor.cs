using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BlackRise.Identity.Middleware;

public class UseCustomErrorModelInterceptor : IValidatorInterceptor
{
    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        return commonContext;
    }

    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext,
        ValidationResult result)
    {
        var failures = result.Errors
            .Select(error => new ValidationFailure(error.PropertyName, SerializeError(error)));

        return new ValidationResult(failures);
    }

    private static string SerializeError(ValidationFailure failure)
    {
        return failure.ErrorMessage;
    }
}
