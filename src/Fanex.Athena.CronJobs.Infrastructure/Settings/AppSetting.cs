using System.Collections.Generic;

namespace Fanex.Athena.CronJobs.Infrastructure.Settings
{
    public class AppSetting
    {
        public int ServerTimeOffSet { get; set; }

        public bool IsMainService { get; set; }

        public IEnumerable<string> MPALicenseeList { get; set; }

        public string Environment { get; set; }
    }
}
