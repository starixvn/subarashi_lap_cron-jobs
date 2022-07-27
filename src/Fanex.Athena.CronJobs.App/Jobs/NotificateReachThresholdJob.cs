using Fanex.Athena.CronJobs.App.Settings;
using Fanex.Athena.CronJobs.Core.Services;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    public class NotificateReachThresholdJob : JobBase
    {
        private const int BATCH_NO = 2000;

        private INotificationService NotificationService { get; }
        private IGlobalSettingService GlobalSettingService { get; }
        private NotificationSetting Setting { get; }

        public NotificateReachThresholdJob(INotificationService notificationService, IGlobalSettingService globalSettingService, IOptions<NotificationSetting> options)
        {
            NotificationService = notificationService;
            GlobalSettingService = globalSettingService;
            Setting = options?.Value;
        }

        public override async Task ExecuteAsync()
        {
            var globalSetting = await GlobalSettingService.GetAllSettingAsync(Setting.MonitorServiceName);
            var lastCheckDate = GlobalSettingService.GetLastCheckReachThresholdTime(globalSetting);
            var testUserId = GlobalSettingService.GetTestIds(globalSetting);
            await NotificationService.NotificateReachThresholdAsync(new Core.Params.NotificationServiceParam
            {
                BatchNo = BATCH_NO,
                IsRunOnlyFirstBatch = false,
                IsTest = Setting.IsTest,
                LastDate = lastCheckDate,
                ListTestUserIds = testUserId
            });
        }
    }
}
