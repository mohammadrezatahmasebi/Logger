using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace AuditLogging;

public class HttpClientAuditLoggingConfiguration : IConfigureOptions<HttpClientFactoryOptions>
{
    public void Configure(HttpClientFactoryOptions options)
    {
        options.HttpMessageHandlerBuilderActions.Add(builder =>
        {
            builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<CorrelationDelegatingHandler>());
            builder.AdditionalHandlers.Add(builder.Services.GetRequiredService<AuditHttpHandler>());
        });
    }
}
