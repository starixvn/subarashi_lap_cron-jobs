namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository;

using System.Threading.Tasks;
using Fanex.Athena.CronJobs.Application.Settings;
using AutoFixture;
using Fanex.Athena.CronJobs.Domain.DTO;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class MatchMonitoringCenterRepositoryTests
{
    private IFixture _fixture;
    private IDynamicRepository _dynamicRepository;
    private IOptions<AppSetting> _appSettings;
    private MatchMonitoringCenterRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _dynamicRepository = Substitute.For<IDynamicRepository>();
        _appSettings = Substitute.For<IOptions<AppSetting>>();
        _repository = new MatchMonitoringCenterRepository(_dynamicRepository, _appSettings);
    }

    [Test]
    public async Task DetectAlertRulesAsync_ShouldCallDynamicRepositoryGetAsync()
    {
        // Act
        await _repository.DetectAlertRulesAsync();

        // Assert
        await _dynamicRepository.Received(1).GetAsync<string>(Arg.Any<DetectAlertRulesCriteria>());
    }

    [Test]
    public async Task UpdateMatchAlertAsync_ShouldCallDynamicRepositoryExecuteAsync()
    {
        // Arrange
        var matchMonitoringData = _fixture.Create<string>();

        // Act
        await _repository.UpdateMatchAlertAsync(matchMonitoringData);

        // Assert
        await _dynamicRepository.Received(1).ExecuteAsync(Arg.Is<UpdateMatchAlertCommand>(c => c.MatchMonitoringData == matchMonitoringData));
    }

    [TestCase("SIT", 1)]
    [TestCase("UAT", 2)]
    [TestCase("PROD", 2)]
    public async Task UpdateAlertHistoryAsync_ShouldCallExecuteAsyncBasedOnEnvironment(string environment, int expectedCalls)
    {
        // Arrange
        var matchMonitoringData = _fixture.Create<string>();
        _appSettings.Value.Returns(new AppSetting { Environment = environment });
        var repository = new MatchMonitoringCenterRepository(_dynamicRepository, _appSettings);

        // Act
        await repository.UpdateAlertHistoryAsync(matchMonitoringData);

        // Assert
        await _dynamicRepository.Received(expectedCalls).ExecuteAsync(Arg.Any<UpdateAlertHistoryCommand>());
    }

    [Test]
    public async Task UpdateAlertHistoryAsync_WhenInProdEnv_ShouldCallExecuteAsyncWithCorrectParameters()
    {
        // Arrange
        const string baseKey = "Task_MatchMonitoring_UpdateAlertHistory";
        var matchMonitoringData = _fixture.Create<string>();
        _appSettings.Value.Returns(new AppSetting { Environment = "PROD" });
        var repository = new MatchMonitoringCenterRepository(_dynamicRepository, _appSettings);

        // Act
        await repository.UpdateAlertHistoryAsync(matchMonitoringData);

        // Assert
        await _dynamicRepository.Received(1).ExecuteAsync(Arg.Is<UpdateAlertHistoryCommand>(c => c.MatchMonitoringData == matchMonitoringData && c.GetSettingKey() == baseKey));
        await _dynamicRepository.Received(1).ExecuteAsync(Arg.Is<UpdateAlertHistoryCommand>(c => c.MatchMonitoringData == matchMonitoringData && c.GetSettingKey() == $"{baseKey}_STANDBY"));
    }

    [Test]
    public async Task GetTriggeredDataSendWebHookAsync_ShouldCallDynamicRepositoryFetchAsync()
    {
        // Arrange
        var licenseeName = _fixture.Create<string>();
        var matchMonitoringData = _fixture.Create<string>();

        // Act
        await _repository.GetTriggeredDataSendWebHookAsync(licenseeName, matchMonitoringData);

        // Assert
        await _dynamicRepository.Received(1).FetchAsync<TriggeredDataSendWebHook>(Arg.Is<GetTriggeredDataSendWebHookCriteria>(c =>
            c.LicenseeName == licenseeName &&
            c.MatchMonitoringData == matchMonitoringData
        ));
    }
}