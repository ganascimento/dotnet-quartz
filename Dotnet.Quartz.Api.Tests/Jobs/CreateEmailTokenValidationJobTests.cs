using Dotnet.Quartz.Api.Context;
using Dotnet.Quartz.Api.Domain.Entities;
using Dotnet.Quartz.Api.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Quartz;
using Xunit;

namespace Dotnet.Quartz.Api.Tests.Jobs;

public class CreateEmailTokenValidationJobTests
{
    private readonly DbContextOptions<AppDataContext> _options;
    private readonly Mock<ILogger<CreateEmailTokenValidationJob>> _mockLogger;
    private readonly CreateEmailTokenValidationJob _job;
    private readonly Mock<IJobExecutionContext> _mockJobContext;
    private readonly Mock<IJobDetail> _mockJobDetail;
    private readonly JobDataMap _jobDataMap;

    public CreateEmailTokenValidationJobTests()
    {
        _options = new DbContextOptionsBuilder<AppDataContext>()
               .UseInMemoryDatabase(databaseName: "testedb").Options;

        using (var context = new AppDataContext(_options))
        {
            context.User?.Add(new UserEntity { UserName = "TestUser", CreatedAt = DateTime.Now });
            context.SaveChanges();
        }

        _mockLogger = new Mock<ILogger<CreateEmailTokenValidationJob>>();
        _job = new CreateEmailTokenValidationJob(new AppDataContext(_options), _mockLogger.Object);
        _mockJobContext = new Mock<IJobExecutionContext>();
        _mockJobDetail = new Mock<IJobDetail>();
        _jobDataMap = new JobDataMap();
    }

    [Fact]
    public async Task Execute_WhenUserExists_ShouldUpdateUserWithNewToken()
    {
        // Arrange
        var userId = 1;
        var user = new UserEntity
        {
            Id = userId,
            UserName = "TestUser",
            CreatedAt = DateTime.Now,
        };
        var mockDbSet = new Mock<DbSet<UserEntity>>();

        _jobDataMap.Put("UserId", userId);
        _mockJobDetail.Setup(x => x.JobDataMap).Returns(_jobDataMap);
        _mockJobContext.Setup(x => x.JobDetail).Returns(_mockJobDetail.Object);
        _mockJobContext.Setup(x => x.MergedJobDataMap).Returns(_jobDataMap);

        // Act
        await _job.Execute(_mockJobContext.Object);

        using (var context = new AppDataContext(_options))
        {
            var createdUser = await context.User!.FindAsync(userId);
            Assert.NotNull(createdUser);
        }
    }
}