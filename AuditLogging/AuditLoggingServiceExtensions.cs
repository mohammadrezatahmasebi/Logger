using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

        return services;
    }

    public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, string connectionString, string databaseName)
    {
        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddScoped<IAuditDbContext, MongoAuditDbContext>();
        return services;
    }

    public static IServiceCollection AddAuditLogReader(this IServiceCollection services)
    {
        services.AddScoped<IAuditLogReader, MongoAuditLogReader>();
        return services;
    }
}
