using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.DTO
{
    public class ServiceSetting
    {
        public string ServiceName { get; set; }

        public string SettingName { get; set; }

        public string SettingValue { get; set; }
    }
}
