namespace TFA.Domain.Monitoring;

internal interface IMonitorRequest
{
    void MonitorSuccess(DomainMetrics metrics);
    void MonitorFailure(DomainMetrics metrics);
}
