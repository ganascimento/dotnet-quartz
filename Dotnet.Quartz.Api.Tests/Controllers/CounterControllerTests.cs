using Dotnet.Quartz.Api.Controllers;
using Dotnet.Quartz.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Dotnet.Quartz.Api.Tests.Controllers;

public class CounterControllerTests
{
    private readonly CounterController _controller;

    public CounterControllerTests()
    {
        _controller = new CounterController();
    }

    [Fact]
    public void GetCounter_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.GetCounter();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void GetCounter_ShouldReturnCounterValue()
    {
        // Act
        var result = _controller.GetCounter() as OkObjectResult;
        var counterValue = result?.Value;

        // Assert
        Assert.NotNull(counterValue);
        Assert.IsType<int>(counterValue);
    }

    [Fact]
    public void GetCounter_ShouldReturnValueFromIncrementCounter()
    {
        // Arrange
        var expectedCounter = IncrementCounter.GetCounter();

        // Act
        var result = _controller.GetCounter() as OkObjectResult;
        var actualCounter = (int)result?.Value!;

        // Assert
        Assert.Equal(expectedCounter, actualCounter);
    }
}