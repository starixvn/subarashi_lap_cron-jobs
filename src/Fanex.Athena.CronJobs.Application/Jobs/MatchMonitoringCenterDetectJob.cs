namespace Fanex.Athena.CronJobs.Application.Jobs;

using System.Threading.Tasks;
using Domain.Logs;
using Domain.Services;

public class MatchMonitoringCenterDetectJob(
    IMatchMonitoringCenterService matchService,
    ILogAggregate logAggregate) : JobBase(logAggregate)
{
    protected override async Task DoJobAsync()
    {
        await matchService.DetectAlertRulesAsync();
    }
}