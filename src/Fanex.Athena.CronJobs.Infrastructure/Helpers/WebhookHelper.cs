namespace Fanex.Athena.CronJobs.Infrastructure.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Domain.DTO;

public static class WebhookHelper
{
    private const string NumberFormat = "#,##0.00";

    private const string DateTimeFormat = "M/d/yyyy HH:mm";

    public static string BuildLarkMessageBody(IEnumerable<TriggeredDataSendWebHook> data)
    {
        var markdownContent = new StringBuilder();

        // English Section
        markdownContent.AppendLine("**[Match Monitoring Notification]**");

        foreach (var item in data)
        {
            markdownContent.AppendLine($"**Monitored Match** ：{item.HomeTeam} - {item.AwayTeam} ({item.LiveStatus})");
            markdownContent.AppendLine($"**Event Date** ：{item.EventDate.ToString(DateTimeFormat)} (GMT+8)");
            markdownContent.AppendLine($"**Negative Profit Forecast** ：{item.ForecastLoss.ToString(NumberFormat, CultureInfo.InvariantCulture)}");
            markdownContent.AppendLine($"**Base Currency** ：{item.BaseCurrency}");
            markdownContent.AppendLine(
                $"**Stake %**：{item.HomeStakePercentage.ToString(NumberFormat, CultureInfo.InvariantCulture)}% : {item.AwayStakePercentage.ToString(NumberFormat, CultureInfo.InvariantCulture)}%");
            markdownContent.AppendLine("**[Reached Criteria]**");
            markdownContent.AppendLine($"**Alert Level Name** ：{item.SettingName}");
            markdownContent.AppendLine($"**Amount Threshold** ：{item.NegativeProfit.ToString(NumberFormat, CultureInfo.InvariantCulture)}");
            markdownContent.AppendLine($"**And/Or**：{item.ConditionType}");
            markdownContent.AppendLine($"**Ratio Threshold**：{item.BetRatio.ToString(NumberFormat, CultureInfo.InvariantCulture)}%");
            markdownContent.AppendLine();
        }

        // Chinese Section
        markdownContent.AppendLine("**[赛事监控通知]**");

        foreach (var item in data)
        {
            markdownContent.AppendLine($"**监控赛事**：{item.HomeTeam} - {item.AwayTeam} ({item.LiveStatus})");
            markdownContent.AppendLine($"**赛事日期**：{item.EventDate.ToString(DateTimeFormat)} (GMT+8)");
            markdownContent.AppendLine($"**负利润预测**：{item.ForecastLoss.ToString(NumberFormat, CultureInfo.InvariantCulture)}");
            markdownContent.AppendLine($"**基础货币**：{item.BaseCurrency}");
            markdownContent.AppendLine(
                $"**投注比例**：{item.HomeStakePercentage.ToString(NumberFormat, CultureInfo.InvariantCulture)}% : {item.AwayStakePercentage.ToString(NumberFormat, CultureInfo.InvariantCulture)}%");
            markdownContent.AppendLine("**[已达标准]**");
            markdownContent.AppendLine($"**警报等级名称** ：{item.SettingName}");
            markdownContent.AppendLine($"**金额门槛** ：{item.NegativeProfit.ToString(NumberFormat, CultureInfo.InvariantCulture)}");
            markdownContent.AppendLine($"**和/或** ：{item.ConditionType}");
            markdownContent.AppendLine($"**比例门槛**：{item.BetRatio.ToString(NumberFormat, CultureInfo.InvariantCulture)}%");
            markdownContent.AppendLine();
        }

        var payload = new
        {
            msg_type = "interactive",
            card = new
            {
                elements = new[]
                {
                    new
                    {
                        tag = "div",
                        text = new
                        {
                            content = markdownContent.ToString(),
                            tag = "lark_md"
                        }
                    }
                }
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        return JsonSerializer.Serialize(payload, options);
    }

    public static LogRequest BuildLogRequest(
        Stopwatch stopwatch,
        string requestBody,
        HttpResponseMessage response,
        string responseBody,
        Uri requestUri,
        Exception ex = null)
    {
        stopwatch.Stop();
        return new LogRequest
        {
            Message = "Sent Lark Webhook",
            Path = requestUri.AbsolutePath,
            Query = requestUri.Query,
            StatusCode = response?.StatusCode,
            IsSuccess = response?.IsSuccessStatusCode,
            DurationMs = stopwatch.ElapsedMilliseconds,
            Request = requestBody,
            Response = responseBody,
            Exception = ex?.Message
        };
    }
}