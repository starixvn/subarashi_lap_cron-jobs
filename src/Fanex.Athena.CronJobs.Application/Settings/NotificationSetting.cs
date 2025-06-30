namespace Fanex.Athena.CronJobs.Application.Settings;

public class NotificationSetting
{
    public const string SettingName = "NotificationSetting";

    public string MonitorServiceName { get; set; }

    public string SuperUUSServiceName { get; set; }

    public bool IsTest { get; set; }
}