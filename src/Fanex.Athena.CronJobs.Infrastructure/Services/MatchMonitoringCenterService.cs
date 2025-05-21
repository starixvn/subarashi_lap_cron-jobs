using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Services
{
    public class MatchMonitoringCenterService : IMatchMonitoringCenterService
    {
        private IMatchMonitoringCenterRepository MatchRepository { get; }
        private AppSetting AppSetting { get; }
        private ILogAggregate LogAggregater { get; }

        public MatchMonitoringCenterService(IMatchMonitoringCenterRepository matchRepository,
            IOptions<AppSetting> options,
            ILogAggregate logAggregater)
        {
            MatchRepository = matchRepository;
            AppSetting = options?.Value;
            LogAggregater = logAggregater;
        }

        public async Task DetectAlertRulesAsync()
        {
            LogAggregater.AppendHeader("Begin Detect Alert Rules Monitoring Center Service");

            string result = await MatchRepository.DetectAlertRulesAsync();

            LogAggregater.AppendLog($"Result: {result}");

            if (!string.IsNullOrEmpty(result))
            {
                LogAggregater.AppendLog("Detected alert rules. Updating match alert and alert history...");

                await MatchRepository.UpdateMatchAlertAsync(result);
                await MatchRepository.UpdateAlertHistoryAsync(result);

                LogAggregater.AppendLog("Match alert and alert history update completed.");
            }
        }
    }
}