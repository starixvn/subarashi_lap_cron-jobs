using System;
using System.Collections.Generic;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class InsertNotificationParam
    {
        public IEnumerable<CustAccumulateParam> Customers { get; set; }

        public int NotificationType { get; set; }

        public DateTime? LastCheckTime { get; set; }

        public DateTime? LastScanSettingDate { get; set; }
    }
}
