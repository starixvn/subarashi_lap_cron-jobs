namespace Fanex.Athena.CronJobs.Infrastructure.Helpers;

public static class CriteriaHelper
{
    public static string GetSettingKey(string spKey, bool isStandby) => spKey + (isStandby ? "_STANDBY" : "");
}