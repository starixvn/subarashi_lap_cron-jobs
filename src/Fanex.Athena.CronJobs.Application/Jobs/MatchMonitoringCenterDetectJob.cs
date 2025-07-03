namespace Fanex.Athena.CronJobs.Application.Jobs;

using System.Threading.Tasks;
using Domain.Logs;
using Domain.Services;
using Hangfire;
using Manual;

public class MatchMonitoringCenterDetectJob(
    IMatchMonitoringCenterService matchService,
    ILogAggregate logAggregate,
    IBackgroundJobClient backgroundJobClient) : JobBase(logAggregate)
{
    protected override async Task DoJobAsync()
    {
        var result = await matchService.DetectAlertRulesAsync();
        backgroundJobClient.Enqueue<LarkWebhookJob>(job => job.Execute(result));
    }
}