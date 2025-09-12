using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace AuditLogging.Tests;

public class AuditEfInterceptorTests
{
    [Fact]
    public void ReaderExecuting_Should_Log_Command()
    {
        // Arrange
        var interceptor = new AuditEfInterceptor();
        var commandMock = new Mock<DbCommand>();
        commandMock.Setup(x => x.CommandText).Returns("SELECT * FROM Test");
        var command = commandMock.Object;
        var eventData = new CommandEventData(new Mock<DbConnection>().Object, command, null, "Test", System.Guid.NewGuid(), false, System.DateTimeOffset.UtcNow);
        var result = new InterceptionResult<DbDataReader>();

        AuditBucketScope.StartScope();

        // Act
        interceptor.ReaderExecuting(command, eventData, result);

        // Assert
        Assert.Single(AuditBucketScope.CurrentBucket.SqlList);
        var sqlAudit = AuditBucketScope.CurrentBucket.SqlList[0];
        Assert.Equal("SELECT * FROM Test", sqlAudit.CommandText);

        AuditBucketScope.EndScope();
    }
}
