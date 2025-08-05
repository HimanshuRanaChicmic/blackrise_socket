using BlackRise.Identity.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace BlackRise.Identity.Middleware;

public class StandardizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public StandardizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        MemoryStream? memoryStream = null;

        try
        {
            memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            // Only process if the response hasn't been written yet and we should standardize
            if (!context.Response.HasStarted && ShouldStandardizeResponse(context))
            {
                await ProcessStandardizedResponse(context, memoryStream, originalBodyStream);
            }
            else
            {
                // Return original response for non-standardized endpoints or if already written
                await CopyOriginalResponse(context, memoryStream, originalBodyStream);
            }
        }
        catch (Exception)
        {
            // If any error occurs, try to restore the original response
            if (memoryStream != null && !memoryStream.CanWrite)
            {
                // Stream is already disposed, just restore the original body
                context.Response.Body = originalBodyStream;
            }
            throw;
        }
        finally
        {
            // Ensure we restore the original body stream
            if (context.Response.Body != originalBodyStream)
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }

    private static async Task ProcessStandardizedResponse(HttpContext context, MemoryStream memoryStream, Stream originalBodyStream)
    {
        try
        {
            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            // Only process if it's a JSON response and hasn't been written
            if (!context.Response.HasStarted && 
                context.Response.ContentType?.Contains("application/json") == true)
            {
                try
                {
                    // Handle success responses (200-299)
                    if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                    {
                        await ProcessSuccessResponse(context, memoryStream, originalBodyStream, responseBody);
                    }
                    // Handle error responses (400+)
                    else if (context.Response.StatusCode >= 400)
                    {
                        await ProcessErrorResponse(context, memoryStream, originalBodyStream, responseBody);
                    }
                    else
                    {
                        // Return original response for other status codes
                        await CopyOriginalResponse(context, memoryStream, originalBodyStream);
                    }
                }
                catch (JsonException)
                {
                    // If parsing fails, return the original response
                    await CopyOriginalResponse(context, memoryStream, originalBodyStream);
                }
            }
            else
            {
                // Return original response for non-JSON responses
                await CopyOriginalResponse(context, memoryStream, originalBodyStream);
            }
        }
        catch (ObjectDisposedException)
        {
            // If the stream is already disposed, just restore the original body
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task ProcessSuccessResponse(HttpContext context, MemoryStream memoryStream, Stream originalBodyStream, string responseBody)
    {
        // Parse the original response
        var originalData = JsonConvert.DeserializeObject(responseBody);
        
        // Check if the original response already has a message property
        string? customMessage = null;
        if (originalData is Newtonsoft.Json.Linq.JObject jObject)
        {
            var messageToken = jObject["message"];
            if (messageToken != null)
            {
                customMessage = messageToken.ToString();
                // Remove the message from the original data since it will be in the standardized response
                jObject.Remove("message");
                originalData = jObject;
            }
        }
        
        // Create standardized response using ResponseBuilder with custom message if provided
        var standardizedResponse = customMessage != null 
            ? ResponseBuilder.Success(originalData, customMessage)
            : ResponseBuilder.Success(originalData);

        await WriteStandardizedResponse(context, originalBodyStream, standardizedResponse);
    }

    private static async Task ProcessErrorResponse(HttpContext context, MemoryStream memoryStream, Stream originalBodyStream, string responseBody)
    {
        try
        {
            // Parse the original error response
            var originalData = JsonConvert.DeserializeObject(responseBody);
            
            string errorMessage = "An error occurred";
            
            // Extract error message from different possible formats
            if (originalData is Newtonsoft.Json.Linq.JObject jObject)
            {
                var errorToken = jObject["error"];
                var messageToken = jObject["message"];
                var validationErrorsToken = jObject["validationErrors"];
                
                if (errorToken != null)
                {
                    errorMessage = errorToken.ToString();
                }
                else if (messageToken != null)
                {
                    errorMessage = messageToken.ToString();
                }
                else if (validationErrorsToken != null)
                {
                    errorMessage = validationErrorsToken.ToString();
                }
            }
            
            // Create standardized error response
            var standardizedResponse = ResponseBuilder.Error<object>(context.Response.StatusCode, errorMessage);
            
            await WriteStandardizedResponse(context, originalBodyStream, standardizedResponse);
        }
        catch (Exception)
        {
            // If parsing fails, return the original response
            await CopyOriginalResponse(context, memoryStream, originalBodyStream);
        }
    }

    private static async Task WriteStandardizedResponse(HttpContext context, Stream originalBodyStream, object standardizedResponse)
    {
        // Serialize the standardized response
        var standardizedJson = JsonConvert.SerializeObject(standardizedResponse, Formatting.Indented);
        var standardizedBytes = Encoding.UTF8.GetBytes(standardizedJson);

        // Reset the response
        context.Response.Body = originalBodyStream;
        context.Response.ContentLength = standardizedBytes.Length;
        context.Response.ContentType = "application/json";

        await context.Response.Body.WriteAsync(standardizedBytes, 0, standardizedBytes.Length);
    }

    private static async Task CopyOriginalResponse(HttpContext context, MemoryStream memoryStream, Stream originalBodyStream)
    {
        try
        {
            if (!context.Response.HasStarted && memoryStream.CanRead)
            {
                memoryStream.Position = 0;
                context.Response.Body = originalBodyStream;
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            else
            {
                // Response already written or stream not readable, just restore original body
                context.Response.Body = originalBodyStream;
            }
        }
        catch (ObjectDisposedException)
        {
            // If the stream is already disposed, just restore the original body
            context.Response.Body = originalBodyStream;
        }
    }

    private static bool ShouldStandardizeResponse(HttpContext context)
    {
        // Check if the endpoint has the StandardizedResponse attribute
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<StandardizedResponseAttribute>() != null)
        {
            return true;
        }

        // Check if the controller has the StandardizedResponse attribute
        var controllerActionDescriptor = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        if (controllerActionDescriptor?.ControllerTypeInfo?.GetCustomAttributes(typeof(StandardizedResponseAttribute), true).Any() == true)
        {
            return true;
        }

        // Check if the action method has the StandardizedResponse attribute
        if (controllerActionDescriptor?.MethodInfo?.GetCustomAttributes(typeof(StandardizedResponseAttribute), true).Any() == true)
        {
            return true;
        }

        return false;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class StandardizedResponseAttribute : Attribute
{
}
