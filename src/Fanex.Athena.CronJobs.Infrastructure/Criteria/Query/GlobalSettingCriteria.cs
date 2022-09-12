using Fanex.Data.Repository;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{

    public class GlobalSettingCriteria : CriteriaBase
    {
        public string ServiceName { get; init; }


        public override string GetSettingKey() => "Acc_WLMonitored_Service_GlobalSettingSel";

        public override bool IsValid() => true;
    }

}
