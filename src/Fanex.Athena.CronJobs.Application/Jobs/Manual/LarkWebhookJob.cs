namespace Fanex.Athena.CronJobs.Application.Jobs.Manual;

using System.Linq;
using System.Threading.Tasks;
using Domain.Services;
using Hangfire;
using Microsoft.Extensions.Options;
using Settings;

public class LarkWebhookJob(
    IMatchMonitoringCenterService matchMonitoringCenterService,
    IOptions<LarkWebhookSetting> larkWebhookSetting)
{
    [DisableConcurrentExecution(60)]
    public async Task Execute(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var webhooks = larkWebhookSetting.Value.Webhooks;
        if (webhooks is null || webhooks.Count == 0)
        {
            return;
        }

        foreach (var webhook in webhooks)
        {
            if (string.IsNullOrEmpty(webhook.WebhookUrl) || string.IsNullOrEmpty(webhook.LicenseeName))
            {
                continue;
            }

            var triggeredData = await matchMonitoringCenterService.GetTriggeredDataSendWebHookAsync(webhook.LicenseeName, data);

            if (triggeredData is null || !triggeredData.Any())
            {
                continue;
            }

            await matchMonitoringCenterService.SendLarkWebhookNotificationAsync(webhook.WebhookUrl, triggeredData);
        }
    }
}