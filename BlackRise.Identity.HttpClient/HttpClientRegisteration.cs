using BlackRise.Identity.Application.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlackRise.Identity.HttpClient;

public static class HttpClientRegisteration
{
    public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        _ = services.AddTransient<IHttpWrapper, HttpWrapper>();
        return services;
    }
}
