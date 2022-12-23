using System;
using System.Collections.Generic;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class InsertBellNotificationParam
    {
        public int MessageType { get; set; }

        public string LicenseeNames { get; set; }

        public string SubjectEn { get; set; }

        public string SubjectCs { get; set; }

        public string ContentEn { get; set; }

        public string ContentCs { get; set; }

        public DateTime EventDate { get; set; }
    }
}