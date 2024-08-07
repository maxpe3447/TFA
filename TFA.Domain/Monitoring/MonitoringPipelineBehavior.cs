using MediatR;
using Microsoft.Extensions.Logging;

namespace TFA.Domain.Monitoring;

internal class MonitoringPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
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

        try
        {
            var result = next.Invoke();
            monitorRequest.MonitorSuccess(metrics);
            return result;
        }
        catch (Exception e)
        {
            monitorRequest.MonitorFailure(metrics);
            logger.LogError(e, "Unhandled error caught while handling command {Command}", request);
            throw;
        }
    }
}
