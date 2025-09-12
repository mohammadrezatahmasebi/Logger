using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuditLogging.Tests;

public class AuditMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_Should_Create_HttpAudit_And_Save_To_Db()
    {
        // Arrange
        var dbContextMock = new Mock<IAuditDbContext>();
        var loggerMock = new Mock<ILogger<AuditMiddleware>>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        var serviceScopeMock = new Mock<IServiceScope>();

        serviceProviderMock.Setup(x => x.GetService(typeof(IAuditDbContext))).Returns(dbContextMock.Object);
        serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
        serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = serviceProviderMock.Object;
        httpContext.Request.Body = new MemoryStream();
        httpContext.Response.Body = new MemoryStream();

        var middleware = new AuditMiddleware(loggerMock.Object);

        RequestDelegate next = (ctx) =>
        {
            return Task.CompletedTask;
        };

        // Act
        await middleware.InvokeAsync(httpContext, next);

        // Assert
        dbContextMock.Verify(x => x.HttpAudits.Add(It.IsAny<HttpAudit>()), Times.Once);
        dbContextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
