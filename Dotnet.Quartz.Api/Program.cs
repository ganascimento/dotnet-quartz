using Dotnet.Quartz.Api.Configuration;
using Dotnet.Quartz.Api.Context;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseInMemoryDatabase("testdb"));

builder.Services.AddQuartz(JobConfigure.ConfigureJobs);
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
