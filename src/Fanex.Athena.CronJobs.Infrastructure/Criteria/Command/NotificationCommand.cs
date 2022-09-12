using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    public class NotificationCommand : NonQueryCommand
    {
        public string CustReached { get; init; }

        public string ServiceName { get; init; }

        public int NotificationType { get; init; }

        public DateTime? LastCheckTime { get; init; }

        public DateTime? LastScanSettingDate { get; init; }

        public override string GetSettingKey() => "Acc_Dep_WLMonitored_Service_ReachHistoryIns";

        public override bool IsValid() => true;
    }
}
