using MediatR;
using TFA.Forum.Domain.Authentication;
using TFA.Forum.Domain.Monitoring;

namespace TFA.Forum.Domain.UseCases.SignOn;

public record SignOnCommand(string Login, string Password) : IRequest<IIdentity>, IMonitorRequest
{
    private const string CounterName = "user.sign-on";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
