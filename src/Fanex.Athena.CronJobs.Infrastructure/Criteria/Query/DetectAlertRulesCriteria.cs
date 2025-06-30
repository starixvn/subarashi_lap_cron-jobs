namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class DetectAlertRulesCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Task_MatchMonitoring_DetectAlertRules";

    public override bool IsValid() => true;
}