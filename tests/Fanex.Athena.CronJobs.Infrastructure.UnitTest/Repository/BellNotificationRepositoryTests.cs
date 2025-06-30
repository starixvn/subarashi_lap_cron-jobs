namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository;

using Criteria.Command;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Params;
using Infrastructure.Repository;
using NSubstitute;
using NUnit.Framework;

public class BellNotificationRepositoryTests
{
    private IDynamicRepository _dynamicRepository;
    private BellNotificationRepository repo;

    [SetUp]
    public void SetUp()
    {
        _dynamicRepository = Substitute.For<IDynamicRepository>();
        repo = new BellNotificationRepository(_dynamicRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _dynamicRepository = null;
    }

    [Test]
    public async Task InsertNotificationAsync_WhenCallWithEmptyParameter_ReceiveParamIsNotValid()
    {
        //Arrange
        var param = new InsertBellNotificationParam();

        //Action
        await repo.InsertNotificationAsync(param);

        //Assert
        await _dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<BellNotificationCommand>(_ => !_.IsValid()));
    }

    [Test]
    public async Task InsertNotificationAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange
        var param = new InsertBellNotificationParam
        {
            MessageType = 1,
            LicenseeNames = "Test",
            SubjectEn = "SubjectEn",
            SubjectCs = "SubjectCs",
            ContentEn = "ContentEn",
            ContentCs = "ContentCs",
            EventDate = DateTime.Now
        };

        //Action
        await repo.InsertNotificationAsync(param);

        //Assert
        await _dynamicRepository.Received().ExecuteAsync(Arg.Is<BellNotificationCommand>(command => IsCorrectNotificationParam(command, param)));
    }

    [Test]
    public async Task InsertMPANotificationAsync_WhenCallWithEmptyParameter_ReceiveParamIsNotValid()
    {
        //Arrange
        var param = new InsertBellNotificationParam();

        //Action
        await repo.InsertMPANotificationAsync(param);

        //Assert
        await _dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<MPABellNotificationCommand>(_ => !_.IsValid()));
    }

    [Test]
    public async Task InsertMPANotificationAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange
        var param = new InsertBellNotificationParam
        {
            MessageType = 2,
            LicenseeNames = "TestMPA",
            SubjectEn = "SubjectEn",
            SubjectCs = "SubjectCs",
            ContentEn = "ContentEn",
            ContentCs = "ContentCs",
            EventDate = DateTime.Now
        };

        //Action
        await repo.InsertMPANotificationAsync(param);

        //Assert
        await _dynamicRepository.Received().ExecuteAsync(Arg.Is<MPABellNotificationCommand>(command => IsCorrectMPANotificationParam(command, param)));
    }

    [Test]
    public async Task GetDWHDailyJobStatusAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange
        var jobId = -1;

        //Action
        await repo.GetDWHDailyJobStatusAsync(jobId);

        //Assert
        await _dynamicRepository.Received()
            .GetAsync<int>(Arg.Is<DWHDailyJobStatusCriteria>(_ => string.CompareOrdinal(_.TaskIDList, jobId.ToString()) == 0));
    }

    [Test]
    public async Task GetSystemSettingAsync_WhenCallWithEmptyParam_ReceiveInvalidParam()
    {
        //Arrange
        var name = string.Empty;

        //Action
        await repo.GetSystemSettingAsync(name);

        //Assert
        await _dynamicRepository.Received()
            .GetAsync<SystemSetting>(Arg.Is<GetSystemSettingCriteria>(_ => !_.IsValid()));
    }

    [Test]
    public async Task GetSystemSettingAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange
        var name = "Test";

        //Action
        await repo.GetSystemSettingAsync(name);

        //Assert
        await _dynamicRepository.Received()
            .GetAsync<SystemSetting>(Arg.Is<GetSystemSettingCriteria>(_ => string.CompareOrdinal(_.Name, name) == 0));
    }

    [Test]
    public async Task GetDailyAbnormalBetAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange

        //Action
        await repo.GetDailyAbnormalBetAsync();

        //Assert
        await _dynamicRepository.Received()
            .FetchAsync<AbnormalBet>(Arg.Is<GetAbnormalBetCriteria>(_ => _.IsValid()));
    }

    [Test]
    public async Task UpdateSystemSettingAsync_WhenCallWithEmptyParam_ReceiveInvalidParam()
    {
        //Arrange
        var param = new SystemSetting
        {
            Name = string.Empty
        };

        //Action
        await repo.UpdateSystemSettingAsync(param);

        //Assert
        await _dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<SystemSettingCommand>(_ => !_.IsValid()));
    }

    [Test]
    public async Task UpdateSystemSettingAsync_WhenCallWithNormalParam_ReceiveCorrectParam()
    {
        //Arrange

        var param = new SystemSetting
        {
            Name = "Test Name",
            Value = "Test Value"
        };

        //Action
        await repo.UpdateSystemSettingAsync(param);

        //Assert
        await _dynamicRepository.Received()
            .ExecuteAsync(Arg.Is<SystemSettingCommand>(_ => IsCorrectSystemSettingParam(_, param)));
    }

    private static bool IsCorrectNotificationParam(BellNotificationCommand command, InsertBellNotificationParam param)
    {
        return command.MessageType.Equals(param.MessageType)
               && command.LicenseeNames.Equals(param.LicenseeNames)
               && command.SubjectEn.Equals(param.SubjectEn)
               && command.SubjectCs.Equals(param.SubjectCs)
               && command.ContentEn.Equals(param.ContentEn)
               && command.ContentCs.Equals(param.ContentCs)
               && command.EventDate.Equals(param.EventDate);
    }

    private static bool IsCorrectMPANotificationParam(MPABellNotificationCommand command, InsertBellNotificationParam param)
    {
        return command.MessageType.Equals(param.MessageType)
               && command.LicenseeNames.Equals(param.LicenseeNames)
               && command.SubjectEn.Equals(param.SubjectEn)
               && command.SubjectCs.Equals(param.SubjectCs)
               && command.ContentEn.Equals(param.ContentEn)
               && command.ContentCs.Equals(param.ContentCs)
               && command.EventDate.Equals(param.EventDate);
    }

    private static bool IsCorrectSystemSettingParam(SystemSettingCommand command, SystemSetting param)
    {
        return command.Name.Equals(param.Name)
               && command.Value.Equals(param.Value);
    }
}