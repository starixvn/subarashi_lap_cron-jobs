namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using System;
using Data.Repository;

public class GetChangeThresholdSettingCriteria : CriteriaBase
{
    public int SportType { get; init; }

    public bool IsDeposit { get; init; }

    public string SuperList { get; init; }

    public bool IsTest { get; init; }

    public DateTime? LastScanDate { get; init; }

    public override string GetSettingKey() => "Acc_WLMonitored_Service_GetChangedSettingList";

    public override bool IsValid() => true;
}