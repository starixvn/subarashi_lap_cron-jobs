namespace Fanex.Athena.CronJobs.Infrastructure.Helpers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        markdownContent.AppendLine("Good Day! This is 1on1 Customer Service. Kindly take note of the below message [请注意以下公告]:");
        markdownContent.AppendLine("**[Match Monitoring Notification]**");
        markdownContent.AppendLine("[#] Monitored Match | Event Date | Negative Profit Forecast | Base Currency | Stake %");
        markdownContent.AppendLine("     **Reached Criteria:** Alert Level Name | Amount Threshold | And/Or | Ratio Threshold");
        markdownContent.AppendLine();
        markdownContent.AppendLine("**[赛事监控通知]**");
        markdownContent.AppendLine("[#] 监控赛事 | 赛事日期 | 负利润预测 | 基础货币 | 投注比例");
        markdownContent.AppendLine("     **已达标准:** 警报等级名称 | 金额门槛 | 和/或 | 比例门槛");
        markdownContent.AppendLine();

        foreach (var item in data)
        {
            var matchDetails =
                $"#{item.RowNum} {item.HomeTeam} - {item.AwayTeam} ({item.LiveStatus}) | " +
                $"{item.EventDate.ToString(DateTimeFormat)} (GMT+8) | " +
                $"{item.ForecastLoss.ToString(NumberFormat)} | {item.BaseCurrency} | " +
                $"{item.HomeStakePercentage.ToString(NumberFormat)}% : {item.AwayStakePercentage.ToString(NumberFormat)}%";

            var criteriaDetails =
                $"     **Reached Criteria:** {item.SettingName} | " +
                $"{item.NegativeProfit.ToString(NumberFormat)} | {item.BaseCurrency} | " +
                $"{item.ConditionType} | {item.BetRatio.ToString(NumberFormat)}%";

            markdownContent.AppendLine(matchDetails);
            markdownContent.AppendLine(criteriaDetails);
            markdownContent.AppendLine();
        }

        markdownContent.Append("Thank you for your kind attention ~");

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