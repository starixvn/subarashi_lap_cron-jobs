namespace Fanex.Athena.CronJobs.Application.Jobs;

using System.Threading.Tasks;
using Domain.Logs;
using Domain.Params;
using Domain.Services;
using Microsoft.Extensions.Options;
using Settings;

public class NotificateChangeSettingJob(
    INotificationService notificationService,
    IGlobalSettingService globalSettingService,
    IOptions<NotificationSetting> options,
    ILogAggregate logAggregate)
    : JobBase(logAggregate)
{
    private NotificationSetting Setting { get; } = options?.Value;

    protected override async Task DoJobAsync()
    {
        var globalSetting = await globalSettingService.GetAllSettingAsync(Setting.MonitorServiceName);
        var testUserIds = await globalSettingService.GetTestIdsAsync(Setting.SuperUUSServiceName);
        var lastScanTime = globalSettingService.GetLastCheckChangeSettingTime(globalSetting);

        await notificationService.NotificateChangeSettingAsync(new NotificationServiceParam
        {
            ListTestUserIds = testUserIds,
            ServiceName = Setting.MonitorServiceName,
            IsTest = Setting.IsTest,
            LastDate = lastScanTime
        });
    }
}