namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class DWHDailyJobStatusCriteria : CriteriaBase
{
    public string TaskIDList { get; set; }

    public override string GetSettingKey() => "DailyJob_CheckTasksStatus";

    public override bool IsValid() => !string.IsNullOrEmpty(TaskIDList);
}