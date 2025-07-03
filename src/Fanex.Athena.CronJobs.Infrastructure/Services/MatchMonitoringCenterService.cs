namespace Fanex.Athena.CronJobs.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.Helpers;
using Application.Settings;
using Domain.DTO;
using Domain.Logs;
using Domain.Repository;
using Domain.Services;
using Helpers;
using Serilog;
using ILogger = Logging.ILogger;

public class MatchMonitoringCenterService(
    IMatchMonitoringCenterRepository matchMonitoringCenterRepository,
    IHttpClientFactory httpClientFactory,
    ILogAggregate logAggregate,
    ILogger logger) : IMatchMonitoringCenterService
{
    public async Task<string> DetectAlertRulesAsync()
    {
        logAggregate.AppendHeader("Begin Detect Alert Rules Monitoring Center Service");

        string result = await matchMonitoringCenterRepository.DetectAlertRulesAsync();

        logAggregate.AppendLog($"Result: {result}");

        if (!string.IsNullOrEmpty(result))
        {
            logAggregate.AppendLog("Detected alert rules. Updating match alert and alert history...");

            await matchMonitoringCenterRepository.UpdateMatchAlertAsync(result);
            await matchMonitoringCenterRepository.UpdateAlertHistoryAsync(result);

            logAggregate.AppendLog("Match alert and alert history update completed.");
        }

        return result;
    }

    public async Task SendLarkWebhookNotificationAsync(string webhookUrl, IEnumerable<TriggeredDataSendWebHook> data)
    {
        var requestBody = WebhookHelper.BuildLarkMessageBody(data);
        var client = httpClientFactory.CreateClient(LarkWebhookSetting.SettingName);

        var stopwatch = Stopwatch.StartNew();
        var uri = new Uri(webhookUrl);
        HttpResponseMessage response = null;
        string responseBody = null;
        Exception caughtException = null;

        try
        {
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            response = await client.PostAsync(uri, content);
            responseBody = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException exception)
        {
            caughtException = exception;
            logAggregate.AppendLog($"Webhook request failed: {exception.Message}");
            var errorMsg = ErrorMessageHelper.GenerateMessage(exception);
            await logger.CriticalAsync(errorMsg, exception);
        }
        finally
        {
            var logRequest = WebhookHelper.BuildLogRequest(stopwatch, requestBody, response, responseBody, uri, caughtException);
            Log.Information("{@LogRequest}", logRequest);
        }
    }

    public Task<IEnumerable<TriggeredDataSendWebHook>> GetTriggeredDataSendWebHookAsync(string licenseeName, string matchMonitoringData)
    {
        return matchMonitoringCenterRepository.GetTriggeredDataSendWebHookAsync(licenseeName, matchMonitoringData);
    }
}