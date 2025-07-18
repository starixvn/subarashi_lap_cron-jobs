namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class GetTriggeredDataSendWebHookCriteria : CriteriaBase
{
    public string LicenseeName { get; set; }

    public string MatchMonitoringData { get; set; }

    public override string GetSettingKey() => "Task_MatchMonitoring_GetTriggeredDataSendWebHook";

    public override bool IsValid() => !string.IsNullOrEmpty(LicenseeName) && !string.IsNullOrEmpty(MatchMonitoringData);
}