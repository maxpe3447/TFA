﻿using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Linq;

namespace TFA.API.Monitoring;

internal static class OpenTelemetryServiceCollectionExtensions
{
    public static IServiceCollection AddApiMetrics(this IServiceCollection services,
        IConfiguration configuration)
    {
        var grpcHost = configuration.GetValue<string>("SearchEngine:Path")!;
        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddMeter("TFA.Forums.Domain")
                //.AddConsoleExporter()
                .AddPrometheusExporter()
                .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
                {
                    Boundaries = [0, 0.05, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 10]
                }))
            .WithTracing(b=>b
                .ConfigureResource(c=>c.AddService("TFA.Forums.API"))
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter += context =>
                        !context.Request.Path.Value!.Contains("metrics", StringComparison.InvariantCultureIgnoreCase) &&
                        !context.Request.Path.Value!.Contains("swagger", StringComparison.InvariantCultureIgnoreCase);

                    options.EnrichWithHttpResponse = (activity, response) =>
                        activity.AddTag("error", response.StatusCode >= 400);
                } )
                .AddEntityFrameworkCoreInstrumentation(cfg=>cfg.SetDbStatementForText = true)
                .AddSource("TFA.Domain")
                .AddHttpClientInstrumentation(options =>
                {
                    options.FilterHttpRequestMessage += message => !message.RequestUri.ToString().Contains(grpcHost);
                })
                .AddGrpcClientInstrumentation()
                .AddJaegerExporter(cfg=>cfg.Endpoint = new Uri(configuration.GetConnectionString("Tracing")!)));

        return services;
    }
}
