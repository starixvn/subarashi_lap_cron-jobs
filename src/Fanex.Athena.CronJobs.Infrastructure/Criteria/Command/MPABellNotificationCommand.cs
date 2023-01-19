using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    public class MPABellNotificationCommand : NonQueryCommand
    {
        public int MessageType { get; set; }

        public string LicenseeNames { get; set; }

        public string SubjectEn { get; set; }

        public string SubjectCs { get; set; }

        public string ContentEn { get; set; }

        public string ContentCs { get; set; }

        public DateTime EventDate { get; set; }

        public override string GetSettingKey() => "N88_MessageIns";

        public override bool IsValid() => MessageType >= 0 && !string.IsNullOrEmpty(LicenseeNames);
    }
}