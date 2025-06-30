namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;

using Data.Repository;

public class GetAbnormalBetCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Lrf_AbnormalBet_GetLicByDate";

    public override bool IsValid() => true;
}