using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace AuditLogging;

public static class AuditLoggingServiceExtensions
{
    public static IServiceCollection AddAuditLogging(this IServiceCollection services, Action<AuditLoggingOptions> configureOptions)
    {
        var options = new AuditLoggingOptions();
        configureOptions(options);

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<CorrelationMiddleware>();
        services.AddScoped<AuditMiddleware>();
        services.AddScoped<AuditHttpHandler>();
        services.AddScoped<CorrelationDelegatingHandler>();

        services.AddSingleton<IConfigureOptions<HttpClientFactoryOptions>, HttpClientAuditLoggingConfiguration>();

        if (options.LogEntityFrameworkCore)
        {
            services.AddSingleton<AuditEfInterceptor>();
        }

        return services;
    }
}
