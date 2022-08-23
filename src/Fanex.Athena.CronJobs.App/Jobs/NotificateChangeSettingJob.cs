using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    public class NotificateChangeSettingJob : JobBase
    {
        private INotificationService NotificationService { get; }
        private IGlobalSettingService GlobalSettingService { get; }
        private NotificationSetting Setting { get; }

        public NotificateChangeSettingJob(INotificationService notificationService
            , IGlobalSettingService globalSettingService
            , IOptions<NotificationSetting> options)
        {
            NotificationService = notificationService;
            GlobalSettingService = globalSettingService;
            Setting = options?.Value;
        }

        public override async Task ExecuteAsync()
        {
            var globalSetting = await GlobalSettingService.GetAllSettingAsync(Setting.MonitorServiceName);
            var testUserIds = GlobalSettingService.GetTestIds(globalSetting);
            var lastScanTime = GlobalSettingService.GetLastCheckChangeSettingTime(globalSetting);

            await NotificationService.NotificateChangeSettingAsync(new Core.Params.NotificationServiceParam
            {
                ListTestUserIds = testUserIds,
                ServiceName = Setting.MonitorServiceName,
                IsTest = Setting.IsTest,
                LastDate = lastScanTime
            });
        }

    }
}
