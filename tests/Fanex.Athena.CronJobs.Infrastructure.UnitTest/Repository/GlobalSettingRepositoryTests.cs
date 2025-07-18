namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository;

using Application.Settings;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class GlobalSettingRepositoryTests
{
    private IOptions<AppSetting> _appSetting;

    private IDynamicRepository _dynamicRepository;

    private GlobalSettingRepository repo;

    [SetUp]
    public void Setup()
    {
        _appSetting = Substitute.For<IOptions<AppSetting>>();
        _appSetting.Value.Returns(new AppSetting());
        _dynamicRepository = Substitute.For<IDynamicRepository>();
        repo = new GlobalSettingRepository(_dynamicRepository, _appSetting);
    }

    [TearDown]
    public void TearDown()
    {
        _dynamicRepository = null;
    }

    [Test]
    public async Task GetSetting_WhenCall_ReceiveCorrectParam()
    {
        //Arrange
        var serviceName = "Test";

        //Action
        await repo.GetSetting(serviceName);

        //Assert
        await _dynamicRepository.Received()
            .FetchAsync<ServiceSetting>(Arg.Is<GlobalSettingCriteria>(_ => string.Equals(_.ServiceName, serviceName, StringComparison.InvariantCulture)));
    }
}