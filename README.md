# Dotnet Quartz

## About

This project was developed to test the Quartz lib, used to schedule jobs

## Resources

- Dotnet 7
- Quartz
- OpenTelemetry
- Aspire
- InMemory DB
- EF Core

## What is Quartz?

Quartz.NET is a scheduling framework for .NET applications that provides robust and flexible job scheduling capabilities. It allows developers to schedule and manage the execution of tasks (jobs) at specified intervals or based on certain triggers. Dotnet Quartz facilitates the creation of complex scheduling scenarios, supporting cron-like expressions for precise control over job execution times.

## Implementation

In Quartz there are many implementations, you can schedule the job to be executed when starting the application or you can schedule the job to be executed once, when calling an endpoint, for example

By default, add the following settings to your `Program.cs`:

```c#
builder.Services.AddQuartz(JobConfigure.ConfigureJobs);
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
```

### Schedule Job

Create a static class to configure your jobs:

```c#
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

        ...
    }
}
```

It's finished, now your job will run every 10 seconds.

### Schedule Job in Controller

Create a static class to configure your jobs:

```c#
public static class JobConfigure
{
    public static class JobConfigure
    {
        public static void ConfigureJobs(this IServiceCollectionQuartzConfigurator options)
        {
            var logginJobKey = JobKey.Create(nameof(IncrementJob));

            options.AddJob<CreateEmailTokenValidationJob>(c => c
                .StoreDurably()
                .WithIdentity(CreateEmailTokenValidationJob.Name));
        }
    }
}
```

In the controller, create the methods like this to call your job:

```c#
private readonly ISchedulerFactory _schedulerFactory;

public UserController(ISchedulerFactory schedulerFactory)
{
    _schedulerFactory = schedulerFactory;
}

...

private async Task ScheludeCreateEmailTokenValidationJob(UserEntity user)
{
    var schedule = await _schedulerFactory.GetScheduler();
    var jobData = new JobDataMap
    {
        { "UserId", user.Id}
    };
    var trigger = TriggerBuilder.Create()
        .ForJob(CreateEmailTokenValidationJob.Name)
        .WithIdentity($"create-email-token-validation-{user.Id}")
        .UsingJobData(jobData)
        //.StartNow()
        .StartAt(DateTime.Now.AddSeconds(5))
        .Build();

    await schedule.ScheduleJob(trigger);
}
```

Cool, now your job will be done in 5 seconds, once.

## Telemetry

To add telemetry packages with this specific version, other version errors may occur:

```c#
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.8.1" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.8.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.8.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.0.0-beta.11" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.8.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Quartz" Version="1.0.0-beta.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" Version="1.8.1" />
```

Configure in `Program.cs`:

```c#
var openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? throw new InvalidDataException("Not found!"));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("QuartzApp"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        metrics.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddQuartzInstrumentation();

        tracing.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
    });

builder.Logging.AddOpenTelemetry(log =>
{
    log.IncludeScopes = true;
    log.IncludeFormattedMessage = true;
    log.AddOtlpExporter(opt => opt.Endpoint = openTelemetryUri);
});
```

Add config to `appsettings.json`:

```json
{
  ...
  "OTEL_EXPORTER_OTLP_ENDPOINT": "http://localhost:4317" <--
}
```

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources: `docker-compose up -d`

In terminal show this:

```console
[+] Running 3/3
 ✔ Network dotnet-quartz_default  Created                                  0.9s
 ✔ Container quartz_app           Started                                  1.6s
 ✔ Container quartz_dashboard     Started                                  2.3s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)
- Aspire Dashboard [click here](http://localhost:18888)

### Stop Application

To stop, run: `docker-compose down`
