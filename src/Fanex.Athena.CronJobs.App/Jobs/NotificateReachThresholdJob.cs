using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
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
            var testUserIds = await GlobalSettingService.GetTestIdsAsync(Setting.SuperUUSServiceName);
            await NotificationService.NotificateReachThresholdAsync(new NotificationServiceParam
            {
                BatchNo = BATCH_NO,
                IsRunOnlyFirstBatch = false,
                IsTest = Setting.IsTest,
                ServiceName = Setting.MonitorServiceName,
                LastDate = lastCheckDate,
                ListTestUserIds = testUserIds
            });
        }
    }
}
