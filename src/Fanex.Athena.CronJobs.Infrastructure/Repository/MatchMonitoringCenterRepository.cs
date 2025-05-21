using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Repository
{
    public class MatchMonitoringCenterRepository : IMatchMonitoringCenterRepository
    {
        private IDynamicRepository DynamicRepository { get; }
        private AppSetting AppSetting { get; }

        public MatchMonitoringCenterRepository(IDynamicRepository dynamicRepository, IOptions<AppSetting> options)
        {
            DynamicRepository = dynamicRepository;
            AppSetting = options?.Value;
        }

        public Task<string> DetectAlertRulesAsync()
        {
            return DynamicRepository.GetAsync<string>(new DetectAlertRulesCriteria());
        }

        public Task UpdateMatchAlertAsync(string matchMonitoringData)
        {
            var command = new UpdateMatchAlertCommand
            {
                MatchMonitoringData = matchMonitoringData
            };

            return DynamicRepository.ExecuteAsync(command);
        }

        public async Task UpdateAlertHistoryAsync(string matchMonitoringData)
        {
            var command53 = new UpdateAlertHistoryCommand("Task_MatchMonitoring_UpdateAlertHistory53")
            {
                MatchMonitoringData = matchMonitoringData
            };

            await DynamicRepository.ExecuteAsync(command53);

            if (IsProductionOrUat())
            {
                var command52 = new UpdateAlertHistoryCommand("Task_MatchMonitoring_UpdateAlertHistory52")
                {
                    MatchMonitoringData = matchMonitoringData
                };

                await DynamicRepository.ExecuteAsync(command52);
            }
        }

        private bool IsProductionOrUat()
        {
            var env = AppSetting?.Environment?.ToUpperInvariant() ?? string.Empty;
            return env.StartsWith("UAT") || env.StartsWith("PRO");
        }
    }
}