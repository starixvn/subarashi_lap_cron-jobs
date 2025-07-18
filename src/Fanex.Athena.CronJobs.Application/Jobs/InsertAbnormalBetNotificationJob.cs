namespace Fanex.Athena.CronJobs.Application.Jobs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Logs;
using Domain.Params;
using Domain.Services;
using Microsoft.Extensions.Options;
using Settings;

internal record MessageTemplate(string SubjectFormat, string BodyFormat);

public class InsertAbnormalBetNotificationJob(
    ILogAggregate logAggregate,
    IBellNotificationService bellNotificationService,
    IOptions<BellNotificationSetting> options,
    IOptions<AppSetting> appOptions)
    : JobBase(logAggregate)
{
    private const int DWHJobDoneStatus = 1;

    private static readonly IDictionary<string, MessageTemplate> Templates = new Dictionary<string, MessageTemplate>
    {
        {
            "en",
            new MessageTemplate(
                SubjectFormat: "Abnormal Bet ({0})",
                BodyFormat: "<div><p><strong>Dear Valued Customers,</strong></p><br>" +
                            "<p>Kindly be informed that the Abnormal Bet Daily report on {0} already exists.</p>" +
                            "<p>Please refer to the Abnormal Bet report for more information.</p><br>" +
                            "<p>Thanks and best regards,</p></div>"
            )
        },
        {
            "cs",
            new MessageTemplate(
                SubjectFormat: "非正规注单 ({0})",
                BodyFormat: "<div><p><strong>尊敬的客户，</strong></p><br>" +
                            "<p>关于 {0} 的每日非正规投注报告已经完成。</p>" +
                            "<p>请参考非正规投注报告以了解更多信息</p><br>" +
                            "<p>谢谢!</p></div>")
        }
    };


    private BellNotificationSetting Setting { get; } = options?.Value;

    private AppSetting AppSetting { get; } = appOptions?.Value;

    protected override async Task DoJobAsync()
    {
        var isJobDone = await bellNotificationService.IsAbnormalBetNotificationJobDone();

        if (!isJobDone && await bellNotificationService.GetDWHDailyJobStatusAsync(Setting.AbnormalBetDWHJobId) == DWHJobDoneStatus)
        {
            var dataList = await bellNotificationService.GetDailyAbnormalBetAsync();

            foreach (var data in dataList)
            {
                var enMessage = GetAbnormalBetMessage(data.CreatedDate, "en");
                var csMessage = GetAbnormalBetMessage(data.CreatedDate, "cs");

                await bellNotificationService.InsertBellNotificationAsync(new InsertBellNotificationParam
                {
                    MessageType = (int)BellNotificationType.AbnormalBet,
                    LicenseeNames = data.LicenseeName,
                    SubjectEn = enMessage.Subject,
                    ContentEn = enMessage.Content,
                    SubjectCs = csMessage.Subject,
                    ContentCs = csMessage.Content,
                    EventDate = data.CreatedDate
                });

                var MPALicenseeList = GetMPALicenseeList(data.LicenseeName);

                if (!string.IsNullOrEmpty(MPALicenseeList))
                {
                    await bellNotificationService.InsertMPABellNotificationAsync(new InsertBellNotificationParam
                    {
                        MessageType = (int)MPABellNotificationType.AbnormalBet,
                        LicenseeNames = MPALicenseeList,
                        SubjectEn = enMessage.Subject,
                        ContentEn = enMessage.Content,
                        SubjectCs = csMessage.Subject,
                        ContentCs = csMessage.Content,
                        EventDate = data.CreatedDate
                    });
                }
            }

            await bellNotificationService.UpdateAbnormalBetNotificationJobDate();
        }
    }

    private (string Subject, string Content) GetAbnormalBetMessage(DateTime eventDate, string langCode)
    {
        if (!Templates.TryGetValue(langCode, out var template))
        {
            return (null, null);
        }

        var formatDate = eventDate.ToString("dd MMM yyyy");
        var subject = string.Format(template.SubjectFormat, formatDate);
        var content = string.Format(template.BodyFormat, formatDate);
        return (subject, content);
    }

    private string GetMPALicenseeList(string licenseeList)
    {
        var MPALicensees = licenseeList.Split(',').Where(licensee => AppSetting.MPALicenseeList.Contains(licensee)).ToList();

        return string.Join(",", MPALicensees);
    }
}