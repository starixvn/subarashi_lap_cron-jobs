namespace Fanex.Athena.CronJobs.Application.Settings;

public class SentrySetting
{
    public const string SettingName = "Sentry";

    public string Environment { get; set; }

    public string Dsn { get; set; }

    public int MaxBreadcrumbs { get; set; }

    public bool SendDefaultPii { get; set; }

    public bool AttachStackTrace { get; set; }

    public bool Debug { get; set; }
}