using BlackRise.Identity.Application.DataTransferObject;
using BlackRise.Identity.Application.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace BlackRise.Identity.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ConvertException(context, ex);
        }
    }

    private static Task ConvertException(HttpContext context, Exception exception)
    {
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

        context.Response.ContentType = "application/json";

        ErrorDto result = new();

        switch (exception)
        {
            case UnAuthorizedException unAuthorizedException:
                httpStatusCode = HttpStatusCode.Unauthorized;
                result = new ErrorDto(unAuthorizedException.Message);
                break;
            case ValidationException validationException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = new ErrorDto(validationException.ValidationErrors);
                break;
            case BadRequestException badRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                result = new ErrorDto(badRequestException.Message);
                break;
            case NotFoundException notFoundException:
                httpStatusCode = HttpStatusCode.NotFound;
                result = new ErrorDto(notFoundException.Message);
                break;
            case Exception internalException:
                httpStatusCode = HttpStatusCode.InternalServerError;
                result = new ErrorDto(internalException.Message);
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;
        return context.Response.WriteAsync(JsonConvert.SerializeObject(result, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }));
    }
}
