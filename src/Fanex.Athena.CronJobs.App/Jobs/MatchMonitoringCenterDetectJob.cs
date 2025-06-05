using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    public class MatchMonitoringCenterDetectJob : JobBase
    {
        private IMatchMonitoringCenterService MatchService { get; }

        public MatchMonitoringCenterDetectJob(IMatchMonitoringCenterService matchService, IGlobalSettingService globalSettingService, IOptions<NotificationSetting> options, ILogAggregate logAggregate)
            : base(logAggregate)
        {
            MatchService = matchService;
        }

        protected override async Task DoJobAsync()
        {
            await MatchService.DetectAlertRulesAsync();
        }
    }
}