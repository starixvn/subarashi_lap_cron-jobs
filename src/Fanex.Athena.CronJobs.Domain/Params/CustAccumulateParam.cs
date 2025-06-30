namespace Fanex.Athena.CronJobs.Domain.Params;

public class CustAccumulateParam
{
    public int CustId { get; set; }

    public decimal AccumWL { get; set; }

    public int SportType { get; set; }

    public decimal WinningThreshold { get; set; }
}