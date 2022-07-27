using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.DTO
{
    public class AccumulateWinloss
    {
        public int CustId { get; set; }

        public decimal AccumWL { get; set; }

        public DateTime SettlementDate { get; set; }

        public DateTime UpdatedTime { get; set; }
    }
}
