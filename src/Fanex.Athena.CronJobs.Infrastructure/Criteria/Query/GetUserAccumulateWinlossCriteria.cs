using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    internal class GetUserAccumulateWinlossCriteria : CriteriaBase
    {
        public string CustId { get; init; }

        public DateTime CurentDate { get; init; }

        public override string GetSettingKey() => "Acc_Dep_WLMonitored_Service_AccumulatedDataSel";

        public override bool IsValid() => true;
    }
}
