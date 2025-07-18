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

public class InsertAbnormalBetNotificationJobTests
{
    private IBellNotificationService BellNotificationService { get; set; }
    private IOptions<BellNotificationSetting> _options { get; set; }
    private IOptions<AppSetting> _appOptions { get; set; }
    private ILogAggregate _logAggregate { get; set; }

    [SetUp]
    public void SetUp()
    {
        BellNotificationService = Substitute.For<IBellNotificationService>();
        _options = Substitute.For<IOptions<BellNotificationSetting>>();
        _appOptions = Substitute.For<IOptions<AppSetting>>();
        _logAggregate = Substitute.For<ILogAggregate>();
        _options.Value.Returns(new BellNotificationSetting { AbnormalBetDWHJobId = 1 });
        _appOptions.Value.Returns(new AppSetting { MPALicenseeList = new[] { "MPATest" } });
    }

    [Test]
    public async Task ExecuteAsync_WhenCall_LogAggregateReceiveCall()
    {
        //Arrange
        var today = DateTime.Now;
        var data = new AbnormalBet { LicenseeName = "Test1,MPATest,Test2", CreatedDate = today };

        var job = new InsertAbnormalBetNotificationJob(_logAggregate, BellNotificationService, _options, _appOptions);
        BellNotificationService.IsAbnormalBetNotificationJobDone().Returns(false);
        BellNotificationService.GetDWHDailyJobStatusAsync(Arg.Any<int>()).Returns(1);
        BellNotificationService.GetDailyAbnormalBetAsync().Returns(new[] { data });

        //Action
        await job.ExecuteAsync();

        //Assert
        await BellNotificationService.Received().InsertBellNotificationAsync(Arg.Is<InsertBellNotificationParam>(_ => IsCorrectNotificationParam(_, data)));
        await BellNotificationService.Received().InsertMPABellNotificationAsync(Arg.Is<InsertBellNotificationParam>(_ =>
            IsCorrectNotificationParam(_, new AbnormalBet { LicenseeName = "MPATest", CreatedDate = today })));
        _logAggregate.Received().SendLog(LogLevel.Information);
    }

    private static bool IsCorrectNotificationParam(InsertBellNotificationParam param, AbnormalBet data)
    {
        return param.LicenseeNames.Equals(data.LicenseeName)
               && param.EventDate.Equals(data.CreatedDate);
    }
}