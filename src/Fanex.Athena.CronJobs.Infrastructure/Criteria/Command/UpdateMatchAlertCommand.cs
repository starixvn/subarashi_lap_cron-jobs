using Fanex.Data.Repository;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    public class UpdateMatchAlertCommand : NonQueryCommand
    {
        public string MatchMonitoringData { get; init; }

        public int ErrCode { get; set; }
        
        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Task_MatchMonitoring_UpdateMatchAlert";

        public override bool IsValid() => !string.IsNullOrEmpty(MatchMonitoringData);
    }
}