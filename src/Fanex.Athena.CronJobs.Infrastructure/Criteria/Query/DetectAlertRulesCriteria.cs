using Fanex.Data.Repository;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    public class DetectAlertRulesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Task_MatchMonitoring_DetectAlertRules";

        public override bool IsValid() => true;
    }
}