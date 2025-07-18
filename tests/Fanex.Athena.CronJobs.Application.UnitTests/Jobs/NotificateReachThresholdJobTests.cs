namespace Fanex.Athena.CronJobs.Application.UnitTests.Jobs;

using Application.Jobs;
using Domain.DTO;
using Domain.Logs;
using Domain.Params;
using Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using Settings;

public class NotificateReachThresholdJobTests
{
    private INotificationService _notificationService { get; set; }
    private IGlobalSettingService _globalSettingService { get; set; }
    private IOptions<NotificationSetting> _options { get; set; }
    private ILogAggregate _logAggregate { get; set; }

    [SetUp]
    public void SetUp()
    {
        _notificationService = Substitute.For<INotificationService>();
        _globalSettingService = Substitute.For<IGlobalSettingService>();
        _options = Substitute.For<IOptions<NotificationSetting>>();
        _logAggregate = Substitute.For<ILogAggregate>();
    }

    [Test]
    public async Task ExecuteAsync_WhenCall_LogAggregateReceiveCall()
    {
        //Arrange
        var setting = new List<ServiceSetting> { new() };
        var date = DateTime.Now;
        var listTestUserId = "1,2";
        _options.Value.ReturnsForAnyArgs(new NotificationSetting());
        _globalSettingService.GetAllSettingAsync(default).ReturnsForAnyArgs(Task.FromResult(setting.AsEnumerable()));
        _globalSettingService.GetTestIdsAsync(default).ReturnsForAnyArgs(Task.FromResult(listTestUserId));
        _globalSettingService.GetLastCheckReachThresholdTime(default).ReturnsForAnyArgs(date);

        var job = new NotificateReachThresholdJob(_notificationService, _globalSettingService, _options, _logAggregate);

        //Action
        await job.ExecuteAsync();

        //Assert
        await _notificationService.Received()
            .NotificateReachThresholdAsync(Arg
                .Is<NotificationServiceParam>(_ => _.LastDate == date
                                                   && _.ListTestUserIds == listTestUserId
                                                   && _.BatchNo == 2000));
        _logAggregate.Received().SendLog(LogLevel.Information);
    }
}