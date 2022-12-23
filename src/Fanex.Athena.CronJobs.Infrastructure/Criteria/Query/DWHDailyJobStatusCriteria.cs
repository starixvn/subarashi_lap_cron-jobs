using Fanex.Data.Repository;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    public class DWHDailyJobStatusCriteria : CriteriaBase
    {
        public string TaskIDList { get; set; }

        public override string GetSettingKey() => "DailyJob_CheckTasksStatus";

        public override bool IsValid() => !string.IsNullOrEmpty(TaskIDList);
    }
}