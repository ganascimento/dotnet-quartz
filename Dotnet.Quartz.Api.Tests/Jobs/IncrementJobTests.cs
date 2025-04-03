using Dotnet.Quartz.Api.Jobs;
using Dotnet.Quartz.Api.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Xunit;

namespace Dotnet.Quartz.Api.Tests.Jobs;

public class IncrementJobTests
{
    private readonly Mock<ILogger<IncrementJob>> _mockLogger;
    private readonly IncrementJob _job;
    private readonly Mock<IJobExecutionContext> _mockJobContext;

    public IncrementJobTests()
    {
        _mockLogger = new Mock<ILogger<IncrementJob>>();
        _job = new IncrementJob(_mockLogger.Object);
        _mockJobContext = new Mock<IJobExecutionContext>();
    }

    [Fact]
    public async Task Execute_ShouldIncrementCounterAndLogMessages()
    {
        // Arrange
        var initialCounter = IncrementCounter.GetCounter();

        // Act
        await _job.Execute(_mockJobContext.Object);

        // Assert
        var finalCounter = IncrementCounter.GetCounter();
        Assert.Equal(initialCounter + 1, finalCounter);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Start increment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Finish increment")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Execute_ShouldHandleConcurrentExecution()
    {
        // Arrange
        var initialCounter = IncrementCounter.GetCounter();

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(_job.Execute(_mockJobContext.Object));
        }
        await Task.WhenAll(tasks);

        // Assert
        var finalCounter = IncrementCounter.GetCounter();
        Assert.Equal(initialCounter + 5, finalCounter);
    }
}