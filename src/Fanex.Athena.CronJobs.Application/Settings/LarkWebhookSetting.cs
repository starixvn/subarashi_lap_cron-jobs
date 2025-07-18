namespace Fanex.Athena.CronJobs.Application.Settings;

using System.Collections.Generic;

public class LarkWebhookSetting
{
    public const string SettingName = "LarkWebhookSetting";

    public List<LarkWebhook> Webhooks { get; set; } = [];

    public string ProxyUrl { get; set; } = string.Empty;
}

public class LarkWebhook
{
    public string WebhookUrl { get; set; }

    public string LicenseeName { get; set; }
}