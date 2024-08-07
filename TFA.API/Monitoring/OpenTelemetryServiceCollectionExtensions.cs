using OpenTelemetry.Metrics;

namespace TFA.API.Monitoring;

internal static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddApiMetrics(this IServiceCollection services)
    {

        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddMeter("TFA.DOMAIN")
                 //.AddConsoleExporter()
                 //.AddOtlpExporter((exporterOptions, metricReaderOptions) =>
                 //{
                 //    exporterOptions.Endpoint = new Uri("http://localhost:9090/api/v1/otlp/v1/metrics");
                 //    exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                 //    metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 1000;
                 //})
                .AddPrometheusExporter()
                .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                }));

        return services;
    }
}
