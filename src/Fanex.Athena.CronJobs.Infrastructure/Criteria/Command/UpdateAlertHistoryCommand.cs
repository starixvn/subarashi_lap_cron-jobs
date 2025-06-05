using Fanex.Data.Repository;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    public class UpdateAlertHistoryCommand : NonQueryCommand
    {
        public string MatchMonitoringData { get; init; }
        
        private readonly string settingKey;

        public int ErrCode { get; set; }
        
        public string ErrMsg { get; set; }

        public UpdateAlertHistoryCommand(string settingKey = "Task_MatchMonitoring_UpdateAlertHistory53")
        {
            this.settingKey = settingKey;
        }

        public override string GetSettingKey() => settingKey;

        public override bool IsValid() => !string.IsNullOrEmpty(MatchMonitoringData);
    }
}