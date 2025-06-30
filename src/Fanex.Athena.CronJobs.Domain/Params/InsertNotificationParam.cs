namespace Fanex.Athena.CronJobs.Domain.Params;

using System;
using System.Collections.Generic;

public class InsertNotificationParam
{
    public IEnumerable<CustAccumulateParam> Customers { get; set; }

    public string ServiceName { get; set; }

    public int NotificationType { get; set; }

    public DateTime? LastCheckTime { get; set; }

    public DateTime? LastScanSettingDate { get; set; }
}