using System;

namespace Fanex.Athena.CronJobs.Core.DTO
{
    public class CustThresholdSetting
    {
        public int CustId { get; set; }

        public decimal WinningThreshold { get; set; }

        public DateTime WinningUpdateTime { get; set; }

        public int SreCommend { get; set; } //SuperUUS ID - Used for testing
    }
}
