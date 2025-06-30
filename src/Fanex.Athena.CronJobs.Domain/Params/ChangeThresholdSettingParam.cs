namespace Fanex.Athena.CronJobs.Domain.Params;

using System;

public class ChangeThresholdSettingParam
{
    public int SportType { get; set; }

    public bool IsDeposit { get; set; }

    public string SuperList { get; set; }

    public bool IsTest { get; set; }

    public DateTime? LastScanDate { get; set; }
}