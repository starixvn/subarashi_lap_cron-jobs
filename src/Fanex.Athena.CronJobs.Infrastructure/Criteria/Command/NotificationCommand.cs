using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    internal class NotificationCommand : NonQueryCommand
    {
        public string CustReached { get; set; }

        public int NotificationType { get; set; }

        public DateTime? LastCheckTime { get; set; }

        public DateTime? LastScanSettingDate { get; set; }

        public override string GetSettingKey() => "Acc_Dep_WLMonitored_Service_ReachHistoryIns";

        public override bool IsValid() => true;
    }
}
