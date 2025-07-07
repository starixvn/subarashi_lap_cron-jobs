namespace Fanex.Athena.CronJobs.Application.UnitTests.Jobs.Manual;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Jobs.Manual;
using Application.Settings;
using AutoFixture;
using Domain.DTO;
using Domain.Services;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class LarkWebhookJobTests
{
    private IFixture _fixture;
    private IMatchMonitoringCenterService _matchMonitoringCenterService;
    private IOptions<LarkWebhookSetting> _larkWebhookSetting;
    private LarkWebhookJob _job;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _matchMonitoringCenterService = Substitute.For<IMatchMonitoringCenterService>();
        _larkWebhookSetting = Substitute.For<IOptions<LarkWebhookSetting>>();
        _job = new LarkWebhookJob(_matchMonitoringCenterService, _larkWebhookSetting);
    }

    [Test]
    public async Task Execute_WhenDataIsNullOrEmpty_ShouldReturn()
    {
        // Act
        await _job.Execute(string.Empty);

        // Assert
        await _matchMonitoringCenterService.DidNotReceiveWithAnyArgs().GetTriggeredDataSendWebHookAsync(default, default);
    }

    [Test]
    public async Task Execute_WhenWebhooksAreNull_ShouldReturn()
    {
        // Arrange
        _larkWebhookSetting.Value.Returns(new LarkWebhookSetting { Webhooks = null });

        // Act
        await _job.Execute("some data");

        // Assert
        await _matchMonitoringCenterService.DidNotReceiveWithAnyArgs().GetTriggeredDataSendWebHookAsync(default, default);
    }

    [Test]
    public async Task Execute_WhenWebhooksAreEmpty_ShouldReturn()
    {
        // Arrange
        _larkWebhookSetting.Value.Returns(new LarkWebhookSetting { Webhooks = new List<LarkWebhook>() });

        // Act
        await _job.Execute("some data");

        // Assert
        await _matchMonitoringCenterService.DidNotReceiveWithAnyArgs().GetTriggeredDataSendWebHookAsync(default, default);
    }

    [Test]
    public async Task Execute_WhenGetTriggeredDataReturnsNull_ShouldNotSendNotification()
    {
        // Arrange
        var webhooks = _fixture.Create<List<LarkWebhook>>();
        _larkWebhookSetting.Value.Returns(new LarkWebhookSetting { Webhooks = webhooks });
        _matchMonitoringCenterService.GetTriggeredDataSendWebHookAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<IEnumerable<TriggeredDataSendWebHook>>(null));

        // Act
        await _job.Execute("some data");

        // Assert
        await _matchMonitoringCenterService.DidNotReceiveWithAnyArgs().SendLarkWebhookNotificationAsync(default, default);
    }

    [Test]
    public async Task Execute_WhenValid_ShouldSendNotification()
    {
        // Arrange
        var webhooks = _fixture.Create<List<LarkWebhook>>();
        var triggeredData = _fixture.Create<List<TriggeredDataSendWebHook>>();

        _larkWebhookSetting.Value.Returns(new LarkWebhookSetting { Webhooks = webhooks });
        _matchMonitoringCenterService.GetTriggeredDataSendWebHookAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Task.FromResult<IEnumerable<TriggeredDataSendWebHook>>(triggeredData));

        // Act
        await _job.Execute("some data");

        // Assert
        foreach (var webhook in webhooks)
        {
            await _matchMonitoringCenterService.Received(1)
                .SendLarkWebhookNotificationAsync(webhook.WebhookUrl, triggeredData);
        }
    }
}