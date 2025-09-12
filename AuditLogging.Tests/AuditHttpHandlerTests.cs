using Moq;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace AuditLogging.Tests;

public class AuditHttpHandlerTests
{
    [Fact]
    public async Task SendAsync_Should_Log_Request_And_Response()
    {
        // Arrange
        var handler = new AuditHttpHandler();
        var innerHandlerMock = new Mock<HttpMessageHandler>();
        handler.InnerHandler = innerHandlerMock.Object;

        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var response = new HttpResponseMessage(HttpStatusCode.OK);

        innerHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", request, ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        AuditBucketScope.StartScope();

        // Act
        await new HttpClient(handler).SendAsync(request, CancellationToken.None);

        // Assert
        Assert.Single(AuditBucketScope.CurrentBucket.OutList);
        var httpCallAudit = AuditBucketScope.CurrentBucket.OutList[0];
        Assert.Equal("GET", httpCallAudit.Method);
        Assert.Equal("https://example.com/", httpCallAudit.Url);
        Assert.Equal(200, httpCallAudit.StatusCode);

        AuditBucketScope.EndScope();
    }
}

public static class MoqExtensions
{
    public static IProtectedMock<HttpMessageHandler> Protected(this Mock<HttpMessageHandler> mock)
    {
        return mock.Protected();
    }
}
