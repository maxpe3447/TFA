using MediatR;
using TFA.Domain.Authentication;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.SignOn;

public record SignOnCommand(string Login, string Password): IRequest<IIdentity>, IMonitorRequest
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
