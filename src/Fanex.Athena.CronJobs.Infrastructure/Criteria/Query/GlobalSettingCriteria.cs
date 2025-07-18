namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;
using Helpers;

public class GlobalSettingCriteria(bool isStandby = false) : CriteriaBase
{
    public string ServiceName { get; init; }

    public override string GetSettingKey() => CriteriaHelper.GetSettingKey("Acc_WLMonitored_Service_GlobalSettingSel", isStandby);

    public override bool IsValid() => true;
}