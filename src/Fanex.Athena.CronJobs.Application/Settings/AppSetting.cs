namespace Fanex.Athena.CronJobs.Application.Settings;

using System.Collections.Generic;

public class AppSetting
{
    public const string SettingName = "App";

    public int ServerTimeOffSet { get; set; }

    public bool IsMainService { get; set; }

    public IEnumerable<string> MPALicenseeList { get; set; }

    public string Environment { get; set; }
}