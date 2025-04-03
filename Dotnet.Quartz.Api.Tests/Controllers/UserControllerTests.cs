using Dotnet.Quartz.Api.Context;
using Dotnet.Quartz.Api.Controllers;
using Dotnet.Quartz.Api.Domain.Dto;
using Dotnet.Quartz.Api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Quartz;
using Xunit;

namespace Dotnet.Quartz.Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly DbContextOptions<AppDataContext> _options;
    private readonly Mock<ISchedulerFactory> _mockSchedulerFactory;
    private readonly Mock<IScheduler> _mockScheduler;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _options = new DbContextOptionsBuilder<AppDataContext>()
            .UseInMemoryDatabase(databaseName: "testdb").Options;

        _mockSchedulerFactory = new Mock<ISchedulerFactory>();
        _mockScheduler = new Mock<IScheduler>();
        _mockSchedulerFactory.Setup(x => x.GetScheduler(default(CancellationToken))).ReturnsAsync(_mockScheduler.Object);

        _controller = new UserController(new AppDataContext(_options), _mockSchedulerFactory.Object);
    }

    [Fact]
    public async Task GetAll_WhenCalled_ReturnsOkWithUsers()
    {
        // Arrange
        using (var context = new AppDataContext(_options))
        {
            context.Database.EnsureDeleted();
            context.User?.Add(new UserEntity { UserName = "TestUser1", CreatedAt = DateTime.Now });
            context.User?.Add(new UserEntity { UserName = "TestUser2", CreatedAt = DateTime.Now });
            await context.SaveChangesAsync();
        }

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var users = Assert.IsType<List<UserEntity>>(okResult.Value);
        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task Create_WithValidUser_ReturnsOkWithCreatedUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto { UserName = "NewUser" };

        // Act
        var result = await _controller.Create(createUserDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var createdUser = Assert.IsType<UserEntity>(okResult.Value);
        Assert.Equal("NewUser", createdUser.UserName);
        Assert.True(createdUser.Id > 0);

        // Verify scheduler was called
        _mockScheduler.Verify(x => x.ScheduleJob(It.IsAny<ITrigger>(), default(CancellationToken)), Times.Once);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var createUserDto = new CreateUserDto { UserName = null };

        // Act
        var result = await _controller.Create(createUserDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}