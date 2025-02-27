using Dotnet.Quartz.Api.Jobs;
using Quartz;

namespace Dotnet.Quartz.Api.Configuration;

public static class JobConfigure
{
    public static void ConfigureJobs(this IServiceCollectionQuartzConfigurator options)
    {
        var logginJobKey = JobKey.Create(nameof(IncrementJob));

        options
            .AddJob<IncrementJob>(logginJobKey)
            .AddTrigger(trigger =>
                trigger
                    .ForJob(logginJobKey)
                    .WithSimpleSchedule(schedule =>
                        schedule
                            .WithIntervalInSeconds(10)
                            .RepeatForever()));

        options.AddJob<CreateEmailTokenValidationJob>(c => c
            .StoreDurably()
            .WithIdentity(CreateEmailTokenValidationJob.Name));
    }
}