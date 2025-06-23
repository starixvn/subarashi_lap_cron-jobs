namespace Fanex.Athena.CronJobs.Infrastructure.Services
{
    using System.Threading.Tasks;
    using Core.Logs;
    using Core.Repository;
    using Core.Services;
    using Microsoft.Extensions.Options;
    using Settings;

    public class MatchMonitoringCenterService : IMatchMonitoringCenterService
    {
        public MatchMonitoringCenterService(
            IMatchMonitoringCenterRepository matchRepository,
            IOptions<AppSetting> options,
            ILogAggregate logAggregate)
        {
            MatchRepository = matchRepository;
            AppSetting = options?.Value;
            LogAggregate = logAggregate;
        }

        private IMatchMonitoringCenterRepository MatchRepository { get; }
        private AppSetting AppSetting { get; }
        private ILogAggregate LogAggregate { get; }

        public async Task DetectAlertRulesAsync()
        {
            LogAggregate.AppendHeader("Begin Detect Alert Rules Monitoring Center Service");

            string result = await MatchRepository.DetectAlertRulesAsync();

            LogAggregate.AppendLog($"Result: {result}");

            if (!string.IsNullOrEmpty(result))
            {
                LogAggregate.AppendLog("Detected alert rules. Updating match alert and alert history...");

                await MatchRepository.UpdateMatchAlertAsync(result);
                await MatchRepository.UpdateAlertHistoryAsync(result);

                LogAggregate.AppendLog("Match alert and alert history update completed.");
            }
        }
    }
}