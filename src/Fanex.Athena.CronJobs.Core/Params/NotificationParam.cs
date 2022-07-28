using System;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class NotificationServiceParam
    {
        public string ServiceName { get; set; }

        public DateTime LastDate { get; set; }

        public bool IsTest { get; set; }

        public string ListTestUserIds { get; set; }

        public bool IsRunOnlyFirstBatch { get; set; }

        public int BatchNo { get; set; }
    }
}
