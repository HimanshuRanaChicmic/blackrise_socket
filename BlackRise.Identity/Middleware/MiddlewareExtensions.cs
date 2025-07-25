namespace BlackRise.Identity.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }

    public static IApplicationBuilder UseLocalizedMessages(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LocalizedMessageMiddleware>();
    }
}
