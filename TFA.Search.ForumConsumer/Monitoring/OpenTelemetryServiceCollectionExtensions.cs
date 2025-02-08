using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Linq;

namespace TFA.Search.ForumConsumer.Monitoring;

internal static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddApiMetrics(this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                //.AddConsoleExporter()
                .AddPrometheusExporter()
                .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                }))
            .WithTracing(b => b
                .ConfigureResource(c => c.AddService("TFA.Search.ForumConsumer"))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter += context =>
                        !context.Request.Path.Value!.Contains("metrics", StringComparison.InvariantCultureIgnoreCase) &&
                        !context.Request.Path.Value!.Contains("swagger", StringComparison.InvariantCultureIgnoreCase);

                    options.EnrichWithHttpResponse = (activity, response) =>
                        activity.AddTag("error", response.StatusCode >= 400);
                })
                .AddSource("ForumSearchConsumer")
                .AddGrpcClientInstrumentation()
                .AddHttpClientInstrumentation()
                .AddJaegerExporter(cfg => cfg.Endpoint = new Uri(configuration.GetConnectionString("Tracing")!)));

        return services;
    }
}
