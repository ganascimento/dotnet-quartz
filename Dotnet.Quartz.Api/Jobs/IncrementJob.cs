using Dotnet.Quartz.Api.Utils;
using Quartz;

namespace Dotnet.Quartz.Api.Jobs;

[DisallowConcurrentExecution]
public class IncrementJob : IJob
{
    private readonly ILogger<IncrementJob> _logger;

    public IncrementJob(ILogger<IncrementJob> logger)
    {
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"{DateTime.UtcNow} | Start increment ...");

        IncrementCounter.SetCounter();
        await Task.Delay(500);

        _logger.LogInformation($"{DateTime.UtcNow} | Finish increment!");
    }
}