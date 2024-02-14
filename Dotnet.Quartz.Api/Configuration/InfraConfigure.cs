using Dotnet.Quartz.Api.Jobs;
using Quartz;

namespace Dotnet.Quartz.Api.Configuration;

public static class InfraConfigure
{
    public static void ConfigureInfra(this IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            var logginJobKey = JobKey.Create(nameof(IncrementJob));

            options
                .AddJob<IncrementJob>(logginJobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(logginJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule
                                .WithIntervalInSeconds(7)
                                .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }
}