using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    internal class GetUserAccumulateWinlossCriteria : CriteriaBase
    {
        public string CustIds { get; init; }

        public DateTime CurrentDate { get; init; }

        public override string GetSettingKey() => "Acc_Dep_WLMonitored_Service_AccumulatedDataSel";

        public override bool IsValid() => true;
    }
}
