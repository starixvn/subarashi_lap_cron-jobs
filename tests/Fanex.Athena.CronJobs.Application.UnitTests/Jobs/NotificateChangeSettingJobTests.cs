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

public class NotificateChangeSettingJobTests
{
    private INotificationService NotificationService { get; set; }
    private IGlobalSettingService GlobalSettingService { get; set; }
    private IOptions<NotificationSetting> _options { get; set; }
    private ILogAggregate _logAggregate { get; set; }

    [SetUp]
    public void SetUp()
    {
        NotificationService = Substitute.For<INotificationService>();
        GlobalSettingService = Substitute.For<IGlobalSettingService>();
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
        GlobalSettingService.GetAllSettingAsync(default).ReturnsForAnyArgs(Task.FromResult(setting.AsEnumerable()));
        GlobalSettingService.GetTestIdsAsync(default).ReturnsForAnyArgs(Task.FromResult(listTestUserId));
        GlobalSettingService.GetLastCheckChangeSettingTime(default).ReturnsForAnyArgs(date);

        var job = new NotificateChangeSettingJob(NotificationService, GlobalSettingService, _options, _logAggregate);

        //Action
        await job.ExecuteAsync();

        //Assert
        await NotificationService.Received()
            .NotificateChangeSettingAsync(Arg.Is<NotificationServiceParam>(_ => _.LastDate == date && _.ListTestUserIds == listTestUserId));
        _logAggregate.Received().SendLog(LogLevel.Information);
    }
}