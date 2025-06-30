namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class GetCustThresholdSettingCriteria : CriteriaBase
{
    public string CustList { get; init; }

    public int SportType { get; init; }

    public bool IsDeposit { get; init; }

    public string SuperList { get; init; }

    public bool IsTest { get; init; }

    public override string GetSettingKey() => "Acc_WLMonitored_Service_GetSettingByCust";

    public override bool IsValid() => true;
}