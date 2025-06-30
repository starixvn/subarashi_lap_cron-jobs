namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using System;
using Data.Repository;
using Helpers;

public class GetLastestAccumulateCriteria(bool isStandby = false) : CriteriaBase
{
    public DateTime LastCheckTIme { get; init; }

    public int BatchNo { get; init; }

    public override string GetSettingKey() => CriteriaHelper.GetSettingKey("Acc_Dep_WLMonitored_Service_LatestAccumulatedDataSel", isStandby);

    public override bool IsValid() => true;
}