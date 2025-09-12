using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace AuditLogging.Tests;

public class CorrelationDelegatingHandlerTests
{
    [Fact]
    public async Task SendAsync_Should_Add_CorrelationId_To_Request_Headers()
    {
        // Arrange
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Items["Correlation-ID"] = "test-correlation-id";
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var handler = new CorrelationDelegatingHandler(httpContextAccessorMock.Object);
        var innerHandlerMock = new Mock<HttpMessageHandler>();
        handler.InnerHandler = innerHandlerMock.Object;

        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        innerHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage());

        // Act
        await new HttpClient(handler).SendAsync(request, CancellationToken.None);

        // Assert
        Assert.True(request.Headers.Contains("X-Correlation-ID"));
        Assert.Equal("test-correlation-id", request.Headers.GetValues("X-Correlation-ID").First());
    }
}
