namespace Fanex.Athena.CronJobs.Application.Jobs;

using System.Threading.Tasks;
using Domain.Logs;
using Domain.Params;
using Domain.Services;
using Microsoft.Extensions.Options;
using Settings;

public class NotificateReachThresholdJob(
    INotificationService notificationService,
    IGlobalSettingService globalSettingService,
    IOptions<NotificationSetting> options,
    ILogAggregate logAggregate)
    : JobBase(logAggregate)
{
    private const int BATCH_NO = 2000;

    private NotificationSetting Setting { get; } = options?.Value;

    protected override async Task DoJobAsync()
    {
        var globalSetting = await globalSettingService.GetAllSettingAsync(Setting.MonitorServiceName);
        var lastCheckDate = globalSettingService.GetLastCheckReachThresholdTime(globalSetting);
        var testUserIds = await globalSettingService.GetTestIdsAsync(Setting.SuperUUSServiceName);
        await notificationService.NotificateReachThresholdAsync(new NotificationServiceParam
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