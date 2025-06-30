namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using System;
using Data.Repository;
using Helpers;

public class GetUserAccumulateWinlossCriteria(bool isStandby = false) : CriteriaBase
{
    public string CustIds { get; init; }

    public DateTime CurrentDate { get; init; }

    public override string GetSettingKey() => CriteriaHelper.GetSettingKey("Acc_Dep_WLMonitored_Service_AccumulatedDataSel", isStandby);

    public override bool IsValid() => true;
}