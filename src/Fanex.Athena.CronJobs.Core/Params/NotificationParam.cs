using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class NotificationServiceParam
    {
        public DateTime LastDate { get; set; }

        public bool IsTest { get; set; }

        public string ListTestUserIds { get; set; }

        public bool IsRunOnlyFirstBatch { get; set; }

        public int BatchNo { get; set; }
    }
}
