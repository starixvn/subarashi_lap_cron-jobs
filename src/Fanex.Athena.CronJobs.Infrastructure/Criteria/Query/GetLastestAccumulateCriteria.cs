using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    internal class GetLastestAccumulateCriteria : CriteriaBase
    {
        public DateTime LastCheckTIme { get; init; }

        public int BatchNo { get; init; }

        public override string GetSettingKey() => "Acc_Dep_WLMonitored_Service_LatestAccumulatedDataSel";

        public override bool IsValid() => true;
    }
}
