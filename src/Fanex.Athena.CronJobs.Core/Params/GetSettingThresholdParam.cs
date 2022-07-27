using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class GetSettingThresholdParam 
    {
        public string CustList { get; set; }

        public int SportType { get; set; }

        public bool IsDeposit { get; set; }

        public string SuperList { get; set; }

        public bool IsTest { get; set; }
    }
}
