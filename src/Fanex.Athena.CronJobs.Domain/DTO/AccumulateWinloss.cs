namespace Fanex.Athena.CronJobs.Domain.DTO;

using System;

public class AccumulateWinloss
{
    public int CustId { get; set; }

    public decimal AccumWL { get; set; }

    public DateTime SettlementDate { get; set; }

    public DateTime UpdatedTime { get; set; }
}