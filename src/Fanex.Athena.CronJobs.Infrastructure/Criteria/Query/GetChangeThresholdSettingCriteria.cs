using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Query
{
    internal class GetChangeThresholdSettingCriteria : CriteriaBase
    {
        public int SportType { get; init; }

        public bool IsDeposit { get; init; }

        public string SuperList { get; init; }

        public bool IsTest { get; init; }

        public DateTime? LastScanDate { get; init; }

        public override string GetSettingKey() => "Acc_WLMonitored_Service_GetChangedSettingList";

        public override bool IsValid() => true;
    }
}
