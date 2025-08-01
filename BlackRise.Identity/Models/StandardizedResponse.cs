using Newtonsoft.Json;
using BlackRise.Identity.Application.Helpers;
using BlackRise.Identity.Application.Utils;

namespace BlackRise.Identity.Models;

public class StandardizedResponse<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    
    [JsonProperty("data")]
    public T? Data { get; set; }
    
    [JsonProperty("message")]
    public string? Message { get; set; }

    public StandardizedResponse(bool success, int statusCode, T? data = default, string? message = null)
    {
        Success = success;
        StatusCode = statusCode;
        Data = data;
        Message = message;
    }
}

public static class ResponseBuilder
{
    /// <summary>
    /// Creates a success response with data and optional message
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    /// <param name="data">The response data</param>
    /// <param name="message">Optional success message</param>
    /// <returns>StandardizedResponse with success=true, statusCode=200</returns>
    public static StandardizedResponse<T> Success<T>(T data, string? message = null)
    {
        var defaultMessage = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.OperationCompletedSuccessfully);
        return new StandardizedResponse<T>(true, 200, data, message ?? defaultMessage);
    }

    /// <summary>
    /// Creates an error response with status code and error message (no data)
    /// </summary>
    /// <typeparam name="T">Type parameter (data will be null)</typeparam>
    /// <param name="statusCode">HTTP status code for the error</param>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with success=false, no data</returns>
    public static StandardizedResponse<T> Error<T>(int statusCode, string message)
    {
        return new StandardizedResponse<T>(false, statusCode, default(T), message);
    }

    /// <summary>
    /// Creates a 400 Bad Request error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with statusCode=400</returns>
    public static StandardizedResponse<object> BadRequest(string message)
    {
        return Error<object>(400, message);
    }

    /// <summary>
    /// Creates a 401 Unauthorized error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with statusCode=401</returns>
    public static StandardizedResponse<object> Unauthorized(string message = null)
    {
        var defaultMessage = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.UnauthorizedAccess);
        return Error<object>(401, message ?? defaultMessage);
    }

    /// <summary>
    /// Creates a 403 Forbidden error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with statusCode=403</returns>
    public static StandardizedResponse<object> Forbidden(string message = null)
    {
        var defaultMessage = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.AccessForbidden);
        return Error<object>(403, message ?? defaultMessage);
    }

    /// <summary>
    /// Creates a 404 Not Found error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with statusCode=404</returns>
    public static StandardizedResponse<object> NotFound(string message = null)
    {
        var defaultMessage = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.ResourceNotFound);
        return Error<object>(404, message ?? defaultMessage);
    }

    /// <summary>
    /// Creates a 500 Internal Server Error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <returns>StandardizedResponse with statusCode=500</returns>
    public static StandardizedResponse<object> InternalServerError(string message = null)
    {
        var defaultMessage = LocalizationHelper.GetLocalizedMessageFromConstantValue(Constants.InternalServerErrorOccurred);
        return Error<object>(500, message ?? defaultMessage);
    }
}
