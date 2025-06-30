namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Services;

using Application.Settings;
using Domain.DTO;
using Domain.Logs;
using Domain.Params;
using Domain.Repository;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class NotificationServiceTests
{
    private const int OffSet = 0;

    private static readonly IEnumerable<CustThresholdSetting>[] EmptyCustThresholdSetting =
    {
        null,
        new List<CustThresholdSetting>()
    };

    private IAccumulateRepository _accumulateRepository;
    private ILogAggregate _logAggregate;
    private INotificationRepository _notificationRepository;
    private IOptions<AppSetting> _options;

    [SetUp]
    public void SetUp()
    {
        _notificationRepository = Substitute.For<INotificationRepository>();
        _accumulateRepository = Substitute.For<IAccumulateRepository>();
        _logAggregate = Substitute.For<ILogAggregate>();
        _options = Substitute.For<IOptions<AppSetting>>();
    }

    [TearDown]
    public void TearDown()
    {
        _notificationRepository = null;
        _accumulateRepository = null;
        _logAggregate = null;
        _options = null;
    }

    [TestCaseSource(nameof(EmptyCustThresholdSetting))]
    public async Task NotificateChangeSettingAsync_WhenHaveNoChangeSettingCust_AccumRepoReceiveEmptyCustString(
        IEnumerable<CustThresholdSetting> custTHresholdSetting)
    {
        //Arrange
        SetupSetting();
        var service = CreateService();
        _notificationRepository.GetCustChangeThresholdSettingAsync(default).ReturnsForAnyArgs(custTHresholdSetting);

        //Action
        await service.NotificateChangeSettingAsync(new NotificationServiceParam());

        //Assert
        await _accumulateRepository.Received().GetUserAccumulateWinlossAsync(Arg.Is<string>(_ => _ == string.Empty), Arg.Any<DateTime>());
    }

    [Test]
    public async Task NotificateChangeSettingAsync_WhenCall_ReceiveCorrectConvertServerTimeOffSet()
    {
        //Arrange
        SetupSetting();
        var service = CreateService();
        IEnumerable<CustThresholdSetting> custSetting = null;
        _notificationRepository.GetCustChangeThresholdSettingAsync(default).ReturnsForAnyArgs(custSetting);
        var now = DateTimeOffset.Now;

        //Action
        await service.NotificateChangeSettingAsync(new NotificationServiceParam());

        //Assert
        await _accumulateRepository.Received().GetUserAccumulateWinlossAsync(Arg.Any<string>(), Arg.Is<DateTime>(_ => CheckDateTimeOffSet(_, now)));
    }

    [Test]
    public async Task NotificateChangeSettingAsync_WhenCallWithNoChangeSettingReach_NotificateRepoCallInsertNotification()
    {
        //Arrange
        SetupSetting();
        var service = CreateService();
        IEnumerable<CustThresholdSetting> custSetting = null;
        _notificationRepository.GetCustChangeThresholdSettingAsync(default).ReturnsForAnyArgs(custSetting);

        //Action
        await service.NotificateChangeSettingAsync(new NotificationServiceParam());

        //Assert
        await _notificationRepository.Received().InsertNotificationAsync(Arg.Any<InsertNotificationParam>());
    }

    [Test]
    public async Task NotificateChangeSettingAsync_WhenCallWithCustReachSetting_InsertNotificateWithCustListNotEmpty()
    {
        //Arrange
        SetupSetting();
        var custSetting = new[]
        {
            new CustThresholdSetting { CustId = 1, WinningThreshold = 1000 }
        };
        var accumWLs = new[]
        {
            new AccumulateWinloss { CustId = 1, AccumWL = 1001 }
        };
        var service = CreateService();
        _notificationRepository.GetCustChangeThresholdSettingAsync(default).ReturnsForAnyArgs(custSetting);
        _accumulateRepository.GetUserAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs);

        //Action
        await service.NotificateChangeSettingAsync(new NotificationServiceParam());

        //Assert
        await _notificationRepository.Received().InsertNotificationAsync(Arg.Is<InsertNotificationParam>(_ => _.Customers.Any()));
    }

    [Test]
    public async Task NotificateChangeSettingAsync_WhenCallWithCustAccumNotReachSetting_InsertNotificateWithEmptyCust()
    {
        //Arrange
        SetupSetting();
        var custSetting = new[]
        {
            new CustThresholdSetting { CustId = 1, WinningThreshold = 1000 }
        };
        var accumWLs = new[]
        {
            new AccumulateWinloss { CustId = 1, AccumWL = 900 }
        };
        var service = CreateService();
        _notificationRepository.GetCustChangeThresholdSettingAsync(default).ReturnsForAnyArgs(custSetting);
        _accumulateRepository.GetUserAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs);

        //Action
        await service.NotificateChangeSettingAsync(new NotificationServiceParam());

        //Assert
        await _notificationRepository.Received().InsertNotificationAsync(Arg.Is<InsertNotificationParam>(_ => !_.Customers.Any()));
    }

    [Test]
    public async Task NotificateReachThresholdAsync_WhenCallWithEmptyAccumWinloss_NotInsertAnyNotificate()
    {
        //Arrange
        SetupSetting();
        var service = CreateService();
        var accumWLs = new List<AccumulateWinloss>();
        _accumulateRepository.GetLastestAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs);

        //Action
        await service.NotificateReachThresholdAsync(new NotificationServiceParam());

        //Assert
        await _notificationRepository.DidNotReceiveWithAnyArgs().InsertNotificationAsync(default);
    }

    [Test]
    public async Task NotificateReachThresholdAsync_WhenCallWithNotEmptyAccumWinloss_CallInsertNotificate()
    {
        SetupSetting();
        var service = CreateService();
        var accumWLs = new[]
        {
            new AccumulateWinloss()
        };
        var param = CreateParam();
        param.BatchNo = accumWLs.Length + 1;
        _accumulateRepository.GetLastestAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs);

        //Action
        await service.NotificateReachThresholdAsync(param);

        //Assert
        await _notificationRepository.ReceivedWithAnyArgs().InsertNotificationAsync(default);
    }

    [Test]
    public async Task NotificateReachThresholdAsync_WhenCallWithNumberOfAccumWinlossMoreThanParamBatchCount_CallInsertNotificateManyTime()
    {
        SetupSetting();
        var service = CreateService();
        var accumWLs = new[]
        {
            new AccumulateWinloss()
        };
        var param = CreateParam();
        param.BatchNo = 1;
        _accumulateRepository.GetLastestAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs, accumWLs, Array.Empty<AccumulateWinloss>());

        //Action
        await service.NotificateReachThresholdAsync(param);

        //Assert
        await _accumulateRepository.ReceivedWithAnyArgs(3).GetLastestAccumulateWinlossAsync(default, default);
        await _notificationRepository.ReceivedWithAnyArgs(2).InsertNotificationAsync(default);
    }

    [Test]
    public async Task NotificateReachThresholdAsync_WhenCallWithAccumWinlossReachThresholdSetting_CallInsertNotificateWithListCustReachNotEmpty()
    {
        //Arrange
        SetupSetting();
        var service = CreateService();
        var accumWLs = new[]
        {
            new AccumulateWinloss { CustId = 1, AccumWL = 1000 },
            new AccumulateWinloss { CustId = 2, AccumWL = 1000 }
        };
        var custSettings = new[]
        {
            new CustThresholdSetting { CustId = 1, WinningThreshold = 100 }
        };
        var param = CreateParam();
        param.BatchNo = accumWLs.Length + 1;
        _accumulateRepository.GetLastestAccumulateWinlossAsync(default, default).ReturnsForAnyArgs(accumWLs);
        _notificationRepository.GetCustThresholdSettingAsync(default).ReturnsForAnyArgs(custSettings);

        //Action
        await service.NotificateReachThresholdAsync(param);

        //Assert
        await _notificationRepository.Received().InsertNotificationAsync(Arg.Is<InsertNotificationParam>(_ => _.Customers.Any()));
    }

    private void SetupSetting()
    {
        _options.Value.ReturnsForAnyArgs(new AppSetting { ServerTimeOffSet = OffSet });
    }

    private static NotificationServiceParam CreateParam()
        => new()
        {
            BatchNo = 2
        };

    private static bool CheckDateTimeOffSet(DateTime convertedDate, DateTimeOffset originDate)
    {
        var diffTimeSpan = convertedDate - originDate.DateTime;
        var originOffSet = originDate.Offset.Hours;
        return diffTimeSpan.Ticks > 0
            ? diffTimeSpan.Hours == OffSet - originOffSet
            : diffTimeSpan.Hours == OffSet - originOffSet + 1 && diffTimeSpan.Minutes == -59 && diffTimeSpan.Seconds == -59;
    }

    private NotificationService CreateService()
        => new(_notificationRepository, _accumulateRepository, _options, _logAggregate);
}