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
                (context.Response.ContentType?.Contains("application/json") == true || 
                 context.Response.ContentType?.Contains("text/plain") == true) && 
                context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                try
                {
                    // Parse the original response
                    object? originalData;
                    
                    if (context.Response.ContentType?.Contains("application/json") == true)
                    {
                        originalData = JsonConvert.DeserializeObject(responseBody);
                    }
                    else
                    {
                        // For text/plain, treat the entire response as the data
                        originalData = responseBody.Trim();
                    }
                    
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

                    // Serialize the standardized response
                    var standardizedJson = JsonConvert.SerializeObject(standardizedResponse, Formatting.Indented);
                    var standardizedBytes = Encoding.UTF8.GetBytes(standardizedJson);

                    // Reset the response
                    context.Response.Body = originalBodyStream;
                    context.Response.ContentLength = standardizedBytes.Length;
                    context.Response.ContentType = "application/json";

                    await context.Response.Body.WriteAsync(standardizedBytes, 0, standardizedBytes.Length);
                }
                catch (JsonException)
                {
                    // If parsing fails, return the original response
                    await CopyOriginalResponse(context, memoryStream, originalBodyStream);
                }
            }
            else
            {
                // Return original response for non-JSON or error responses
                await CopyOriginalResponse(context, memoryStream, originalBodyStream);
            }
        }
        catch (ObjectDisposedException)
        {
            // If the stream is already disposed, just restore the original body
            context.Response.Body = originalBodyStream;
        }
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
