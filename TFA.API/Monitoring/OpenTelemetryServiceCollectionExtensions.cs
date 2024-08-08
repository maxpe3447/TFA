using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace TFA.API.Monitoring;

internal static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddApiMetrics(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddMeter("TFA.DOMAIN")
                //.AddConsoleExporter()
                .AddPrometheusExporter()
                .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                }))
            .WithTracing(b=>b
                .ConfigureResource(c=>c.AddService("TFA"))
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(cfg=>cfg.SetDbStatementForText = true)
                .AddSource("TFA.Domain")
                .AddJaegerExporter(cfg=>cfg.Endpoint = new Uri(configuration.GetConnectionString("Tracing")!)));

        return services;
    }
}
