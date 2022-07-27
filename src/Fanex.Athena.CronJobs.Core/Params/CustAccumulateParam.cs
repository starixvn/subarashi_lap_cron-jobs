

namespace Fanex.Athena.CronJobs.Core.Params
{
    public class CustAccumulateParam
    {
        public int CustId { get; set; }

        public decimal AccumWL { get; set; }

        public int SportType { get; set; }

        public decimal WinningThreshold { get; set; }
    }
}
