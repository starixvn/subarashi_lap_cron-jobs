namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class GetSystemSettingCriteria : CriteriaBase
{
    public string Name { get; set; }

    public override string GetSettingKey() => "Lrf_SystemSettingsByNameSel";

    public override bool IsValid() => !string.IsNullOrEmpty(Name);
}