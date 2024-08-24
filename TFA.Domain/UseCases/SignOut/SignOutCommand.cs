using MediatR;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.SignOut;

public record SignOutCommand() : IRequest, IMonitorRequest
{
    private const string CounterName = "user.sign-out";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
