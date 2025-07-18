namespace Fanex.Athena.CronJobs.Application.Settings;

public class RedisSetting
{
    public const string SettingName = "Redis";

    public string Server { get; set; }

    public int Port { get; set; }

    public string Pass { get; set; }

    public int Database { get; set; }

    public string AppName { get; set; }

    public string KeepAlive { get; set; }

    public string ProtectionDataKey { get; set; }
}