namespace Fanex.Athena.CronJobs.Domain.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;

public interface IMatchMonitoringCenterService
{
    Task<string> DetectAlertRulesAsync();

    Task SendLarkWebhookNotificationAsync(string webhookUrl, IEnumerable<TriggeredDataSendWebHook> data);

    Task<IEnumerable<TriggeredDataSendWebHook>> GetTriggeredDataSendWebHookAsync(string licenseeName, string matchMonitoringData);
}