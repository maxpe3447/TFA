using MediatR;
using TFA.Domain.Authentication;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.SignIn;

public record SignInCommand(string Login, string Password) 
    : IRequest<(IIdentity identity, string token)>, IMonitorRequest
{
    private const string CounterName = "user.sign-in";

    void IMonitorRequest.MonitorFailure(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(false));
    }

    void IMonitorRequest.MonitorSuccess(DomainMetrics metrics)
    {
        metrics.Increment(CounterName, 1, DomainMetrics.ResultTags(true));

    }
}
