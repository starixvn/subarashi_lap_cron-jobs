namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;

using System;
using Data.Repository;
using Helpers;

public class NotificationCommand(bool isStandby) : NonQueryCommand
{
    public string CustReached { get; init; }

    public string ServiceName { get; init; }

    public int NotificationType { get; init; }

    public DateTime? LastCheckTime { get; init; }

    public DateTime? LastScanSettingDate { get; init; }

    public override string GetSettingKey() => CriteriaHelper.GetSettingKey("Acc_Dep_WLMonitored_Service_ReachHistoryIns", isStandby);

    public override bool IsValid() => true;
}