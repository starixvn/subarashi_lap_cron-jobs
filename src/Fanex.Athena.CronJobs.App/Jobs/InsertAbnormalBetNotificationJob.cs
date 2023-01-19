using Fanex.Athena.CronJobs.Core.Enums;
using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    public class InsertAbnormalBetNotificationJob : JobBase
    {
        private IBellNotificationService BellNotificationService { get; }

        private BellNotificationSetting Setting { get; }

        private AppSetting AppSetting { get; }

        private const int DWHJobDoneStatus = 1;

        private readonly IDictionary<string, string> _subjectFormat = new Dictionary<string, string>
        {
            {"en", "Abnormal Bet ({0})"},
            {"cs", "非正规注单 ({0})"},
        };

#pragma warning disable S103 // Lines should not be too long

        private readonly IDictionary<string, string> _contentFormat = new Dictionary<string, string>
        {
            {"en", "<div><p><strong>Dear Valued Customers,</strong></p><br><p>Kindly be informed that the Abnormal Bet Daily report on {0} already exists.</p><p>Please refer to the Abnormal Bet report for more information.</p><br><p>Thanks and best regards,</p></div>"},
            {"cs", "<div><p><strong>尊敬的客户，</strong></p><br><p>关于 {0} 的每日非正规投注报告已经完成。</p><p>请参考非正规投注报告以了解更多信息</p><br><p>谢谢!</p></div>"},
        };

#pragma warning restore S103 // Lines should not be too long

        public InsertAbnormalBetNotificationJob(ILogAggregate logAggregate,
            IBellNotificationService bellNotificationService,
            IOptions<BellNotificationSetting> options,
            IOptions<AppSetting> appOptions)
            : base(logAggregate)
        {
            BellNotificationService = bellNotificationService;
            Setting = options?.Value;
            AppSetting = appOptions?.Value;
        }

        protected override async Task DoJobAsync()
        {
            var isJobDone = await BellNotificationService.IsAbnormalBetNotificationJobDone();

            if (!isJobDone && await BellNotificationService.GetDWHDailyJobStatusAsync(Setting.AbnormalBetDWHJobId) == DWHJobDoneStatus)
            {
                var dataList = await BellNotificationService.GetDailyAbnormalBetAsync();

                foreach (var data in dataList)
                {
                    await BellNotificationService.InsertBellNotificationAsync(new InsertBellNotificationParam
                    {
                        MessageType = (int)BellNotificationType.AbnormalBet,
                        LicenseeNames = data.LicenseeName,
                        SubjectEn = GetAbnormalBetSubject(data.CreatedDate, "en"),
                        SubjectCs = GetAbnormalBetSubject(data.CreatedDate, "cs"),
                        ContentEn = GetAbnormalBetContent(data.CreatedDate, "en"),
                        ContentCs = GetAbnormalBetContent(data.CreatedDate, "cs"),
                        EventDate = data.CreatedDate
                    });

                    var MPALicenseeList = GetMPALicenseeList(data.LicenseeName);

                    if (!string.IsNullOrEmpty(MPALicenseeList))
                    {
                        await BellNotificationService.InsertMPABellNotificationAsync(new InsertBellNotificationParam
                        {
                            MessageType = (int)MPABellNotificationType.AbnormalBet,
                            LicenseeNames = MPALicenseeList,
                            SubjectEn = GetAbnormalBetSubject(data.CreatedDate, "en"),
                            SubjectCs = GetAbnormalBetSubject(data.CreatedDate, "cs"),
                            ContentEn = GetAbnormalBetContent(data.CreatedDate, "en"),
                            ContentCs = GetAbnormalBetContent(data.CreatedDate, "cs"),
                            EventDate = data.CreatedDate
                        });
                    }
                }

                await BellNotificationService.UpdateAbnormalBetNotificationJobDate();
            }
        }

        private string GetAbnormalBetContent(DateTime eventDate, string langCode)
        {
            return _contentFormat.ContainsKey(langCode) ? string.Format(_contentFormat[langCode], eventDate.ToString("dd MMM yyyy")) : null;
        }

        private string GetAbnormalBetSubject(DateTime eventDate, string langCode)
        {
            return _subjectFormat.ContainsKey(langCode) ? string.Format(_subjectFormat[langCode], eventDate.ToString("dd MMM yyyy")) : null;
        }

        private string GetMPALicenseeList(string licenseeList)
        {
            var MPALicensees = new List<string>();

            foreach (var licensee in licenseeList.Split(','))
            {
                if (AppSetting.MPALicenseeList.Contains(licensee))
                {
                    MPALicensees.Add(licensee);
                }
            }

            return string.Join(",", MPALicensees);
        }
    }
}