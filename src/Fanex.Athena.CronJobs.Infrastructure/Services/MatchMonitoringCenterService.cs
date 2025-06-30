namespace Fanex.Athena.CronJobs.Infrastructure.Services;

using System.Threading.Tasks;
using Domain.Logs;
using Domain.Repository;
using Domain.Services;

public class MatchMonitoringCenterService(
    IMatchMonitoringCenterRepository matchRepository,
    ILogAggregate logAggregate)
    : IMatchMonitoringCenterService
{
    public async Task DetectAlertRulesAsync()
    {
        logAggregate.AppendHeader("Begin Detect Alert Rules Monitoring Center Service");

        string result = await matchRepository.DetectAlertRulesAsync();

        logAggregate.AppendLog($"Result: {result}");

        if (!string.IsNullOrEmpty(result))
        {
            logAggregate.AppendLog("Detected alert rules. Updating match alert and alert history...");

            await matchRepository.UpdateMatchAlertAsync(result);
            await matchRepository.UpdateAlertHistoryAsync(result);

            logAggregate.AppendLog("Match alert and alert history update completed.");
        }
    }
}