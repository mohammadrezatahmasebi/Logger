using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AuditLogging.Tests;

public class CorrelationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Add_CorrelationId_To_Request_And_Response()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CorrelationMiddleware>>();
        var httpContext = new DefaultHttpContext();
        var middleware = new CorrelationMiddleware(loggerMock.Object);

        RequestDelegate next = (ctx) =>
        {
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(httpContext, next);

        // Assert
        Assert.True(httpContext.Items.ContainsKey("Correlation-ID"));
        Assert.True(httpContext.Response.Headers.ContainsKey("X-Correlation-ID"));
    }
}
