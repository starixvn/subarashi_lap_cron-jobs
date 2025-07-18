namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;

using Data.Repository;
using Helpers;

public class UpdateAlertHistoryCommand(bool isStandby = false) : NonQueryCommand
{
    public string MatchMonitoringData { get; init; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => CriteriaHelper.GetSettingKey("Task_MatchMonitoring_UpdateAlertHistory", isStandby);

    public override bool IsValid() => !string.IsNullOrEmpty(MatchMonitoringData);
}