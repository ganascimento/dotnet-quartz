using Dotnet.Quartz.Api.Context;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Dotnet.Quartz.Api.Jobs;

public sealed class CreateEmailTokenValidationJob : IJob
{
    public static readonly string Name = nameof(CreateEmailTokenValidationJob);

    private readonly AppDataContext _context;
    private readonly ILogger<CreateEmailTokenValidationJob> _logger;

    public CreateEmailTokenValidationJob(AppDataContext context, ILogger<CreateEmailTokenValidationJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{DateTime.UtcNow} | Start create token ...");

        var jobData = context.MergedJobDataMap;
        var userId = jobData.GetInt("UserId");

        _logger.LogInformation($"{DateTime.UtcNow} | Creating token to user {userId}");

        var user = await _context.User!.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
            throw new InvalidDataException("User not found!");

        user.EmailValidationToken = Guid.NewGuid().ToString();
        user.SendValidationTokenDate = DateTime.Now;

        _context.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"{DateTime.UtcNow} | Finish create token ...");
    }
}