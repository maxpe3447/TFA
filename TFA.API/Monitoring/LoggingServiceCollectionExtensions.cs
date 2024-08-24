using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics;

namespace TFA.API.Monitoring;

public static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment)
    {

        var loggingLevelSwitch = new LoggingLevelSwitch();
        services.AddSingleton(loggingLevelSwitch);


        return services.AddLogging(b => b
            .Configure(opt => opt.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId)
            .AddSerilog(new LoggerConfiguration()
    .MinimumLevel.ControlledBy(loggingLevelSwitch)
    .Enrich.WithProperty("Application", "TFA.API")
    .Enrich.WithProperty("Environment", webHostEnvironment.EnvironmentName)
    .Enrich.With<TracingContextEnricher>()
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        //.WriteTo.OpenSearch(
        //configuration.GetConnectionString("Logs"),
        //"forum-logs-{0:yyyy.MM.dd}")
        .WriteTo.GrafanaLoki(
            configuration.GetConnectionString("Logs-loki")!,
            propertiesAsLabels: [
                "level",
                "Environment",
                "Application",
                "SourceContext"//get type of the method where we had an error
                ],
            leavePropertiesIntact: true)
        )
    .WriteTo.Logger(lc => lc
        .Filter.ByExcluding(Matching.FromSource("Microsoft"))
        .WriteTo.Console())
    .CreateLogger()));
    }
}
internal class TracingContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId));

    }
}