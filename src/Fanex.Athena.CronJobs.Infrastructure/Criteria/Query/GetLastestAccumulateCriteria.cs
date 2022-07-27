using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
