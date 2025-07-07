namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Services;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Application.Settings;
using AutoFixture;
using Domain.DTO;
using Domain.Logs;
using Domain.Repository;
using Infrastructure.Services;
using Logging;
using NSubstitute;
using NUnit.Framework;

public class MatchMonitoringCenterServiceTests
{
    private IFixture _fixture;
    private HttpClient _httpClient;
    private IHttpClientFactory _httpClientFactory;
    private ILogAggregate _logAggregate;
    private ILogger _logger;
    private MockHttpMessageHandler _mockHttpMessageHandler;
    private IMatchMonitoringCenterRepository _repository;
    private MatchMonitoringCenterService _service;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _repository = Substitute.For<IMatchMonitoringCenterRepository>();
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _logAggregate = Substitute.For<ILogAggregate>();
        _logger = Substitute.For<ILogger>();
        _mockHttpMessageHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_mockHttpMessageHandler);

        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_httpClient);

        _service = new MatchMonitoringCenterService(_repository, _httpClientFactory, _logAggregate, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _httpClient.Dispose();
        _mockHttpMessageHandler.Dispose();
    }

    [Test]
    public async Task DetectAlertRulesAsync_WithNonEmptyResult_ShouldUpdateAndReturnResult()
    {
        // Arrange
        var expectedResult = _fixture.Create<string>();
        _repository.DetectAlertRulesAsync().Returns(Task.FromResult(expectedResult));

        // Act
        var actualResult = await _service.DetectAlertRulesAsync();

        // Assert
        Assert.That(actualResult, Is.EqualTo(expectedResult));
        await _repository.Received(1).DetectAlertRulesAsync();
        await _repository.Received(1).UpdateMatchAlertAsync(expectedResult);
        await _repository.Received(1).UpdateAlertHistoryAsync(expectedResult);
    }

    [TestCase(null)]
    [TestCase("")]
    public async Task DetectAlertRulesAsync_WithEmptyOrNullResult_ShouldNotUpdateAndReturnResult(string? repositoryResult)
    {
        // Arrange
        _repository.DetectAlertRulesAsync().Returns(Task.FromResult(repositoryResult));

        // Act
        var actualResult = await _service.DetectAlertRulesAsync();

        // Assert
        Assert.That(actualResult, Is.EqualTo(repositoryResult));
        await _repository.Received(1).DetectAlertRulesAsync();
        await _repository.DidNotReceive().UpdateMatchAlertAsync(Arg.Any<string>());
        await _repository.DidNotReceive().UpdateAlertHistoryAsync(Arg.Any<string>());
    }

    [Test]
    public async Task SendLarkWebhookNotificationAsync_WhenCalled_UsesCorrectNamedHttpClient()
    {
        // Arrange
        var webhookUrl = "http://fake-webhook.com";
        var data = _fixture.CreateMany<TriggeredDataSendWebHook>();

        // Act
        await _service.SendLarkWebhookNotificationAsync(webhookUrl, data);

        // Assert
        _httpClientFactory.Received(1).CreateClient(LarkWebhookSetting.SettingName);
        Assert.That(_mockHttpMessageHandler.NumberOfCalls, Is.EqualTo(1));
    }

    [Test]
    public async Task SendLarkWebhookNotificationAsync_WhenRequestFails_ShouldLogCriticalError()
    {
        // Arrange
        var webhookUrl = "http://fake-webhook.com";
        var data = _fixture.CreateMany<TriggeredDataSendWebHook>();
        var exception = new HttpRequestException("Request failed");

        _mockHttpMessageHandler.OnSend = (_, _) => throw exception;

        // Act
        await _service.SendLarkWebhookNotificationAsync(webhookUrl, data);

        // Assert
        _logAggregate.Received(1).AppendLog($"Webhook request failed: {exception.Message}");
        await _logger.Received(1).CriticalAsync(Arg.Any<string>(), exception);
    }

    [Test]
    public async Task GetTriggeredDataSendWebHookAsync_WhenCalled_ShouldCallRepositoryAndReturnData()
    {
        // Arrange
        var licenseeName = _fixture.Create<string>();
        var matchData = _fixture.Create<string>();
        var expectedData = _fixture.Create<List<TriggeredDataSendWebHook>>();
        _repository.GetTriggeredDataSendWebHookAsync(licenseeName, matchData)
            .Returns(Task.FromResult<IEnumerable<TriggeredDataSendWebHook>>(expectedData));

        // Act
        var actualData = await _service.GetTriggeredDataSendWebHookAsync(licenseeName, matchData);

        // Assert
        Assert.That(actualData, Is.EqualTo(expectedData));
        await _repository.Received(1).GetTriggeredDataSendWebHookAsync(licenseeName, matchData);
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    public int NumberOfCalls { get; private set; }

    public Action<HttpRequestMessage, CancellationToken> OnSend { get; set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        NumberOfCalls++;

        OnSend?.Invoke(request, cancellationToken);

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}