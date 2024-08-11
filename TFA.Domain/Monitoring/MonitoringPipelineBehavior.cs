using MediatR;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace TFA.Domain.Monitoring;

internal abstract class MonitoringPipelineBehavior
{
    protected static readonly TextMapPropagator Propagation = Propagators.DefaultTextMapPropagator;

}

internal class MonitoringPipelineBehavior<TRequest, TResponse> :MonitoringPipelineBehavior, IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly DomainMetrics metrics;
    private readonly ILogger<MonitoringPipelineBehavior<TRequest, TResponse>> logger;

    public MonitoringPipelineBehavior(
        DomainMetrics domainMetrics,
        ILogger<MonitoringPipelineBehavior<TRequest, TResponse>> logger
)
    {
        this.metrics = domainMetrics;
        this.logger = logger;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IMonitorRequest monitorRequest)
        {
            return next.Invoke();
        }
        using var activity = DomainMetrics.ActivitySource.StartActivity(
            "usecase", ActivityKind.Internal,default(ActivityContext));
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
        activity.AddTag("tfa.command", request.GetType().Name);
        try
        {
            var result = next.Invoke();
            monitorRequest.MonitorSuccess(metrics);
            activity.AddTag("error", false);
            logger.LogInformation("Command successfully handled {{Command}}", request);

            return result;
        }
        catch (Exception e)
        {
            monitorRequest.MonitorFailure(metrics);
            activity.AddTag("error", true);
            logger.LogError(e, "Unhandled error caught while handling command {Command}", request);
            throw;
        }
    }
}
