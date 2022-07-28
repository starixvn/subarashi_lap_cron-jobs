using System;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class ChangeThresholdSettingParam
    {
        public int SportType { get; set; }

        public bool IsDeposit { get; set; }

        public string SuperList { get; set; }

        public bool IsTest { get; set; }

        public DateTime? LastScanDate { get; set; }
    }
}
