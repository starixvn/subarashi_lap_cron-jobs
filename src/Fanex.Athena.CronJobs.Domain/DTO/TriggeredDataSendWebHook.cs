namespace Fanex.Athena.CronJobs.Domain.DTO;

using System;

public class TriggeredDataSendWebHook
{
    public long RowNum { get; set; }

    public string HomeTeam { get; set; }

    public string AwayTeam { get; set; }

    public string LiveStatus { get; set; }

    public DateTime EventDate { get; set; }

    public decimal ForecastLoss { get; set; }

    public string BaseCurrency { get; set; }

    public decimal HomeStakePercentage { get; set; }

    public decimal AwayStakePercentage { get; set; }

    public string SettingName { get; set; }

    public decimal NegativeProfit { get; set; }

    public string ConditionType { get; set; }

    public decimal BetRatio { get; set; }
}