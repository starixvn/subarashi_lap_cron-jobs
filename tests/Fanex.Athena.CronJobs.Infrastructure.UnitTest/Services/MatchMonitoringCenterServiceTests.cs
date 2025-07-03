namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Services;

using System.Net;
using Application.Settings;
using Domain.DTO;
using Domain.Logs;
using Domain.Repository;
using Domain.Services;
using Infrastructure.Services;
using Logging;
using NSubstitute;
using NUnit.Framework;

public class MatchMonitoringCenterServiceTests
{
    private HttpClient _httpClient;
    private IHttpClientFactory _httpClientFactory;
    private ILogAggregate _logAggregate;
    private ILogger _logger;
    private MockHttpMessageHandler _mockHttpMessageHandler;
    private IMatchMonitoringCenterRepository _repository;
    private IMatchMonitoringCenterService _service;

    [SetUp]
    public void SetUp()
    {
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
    public async Task SendLarkWebhookNotificationAsync_WhenCalled_UsesCorrectNamedHttpClient()
    {
        // Arrange
        var webhookUrl = "http://fake-webhook.com";
        var data = new TriggeredDataSendWebHook[] { new() };

        // Act
        await _service.SendLarkWebhookNotificationAsync(webhookUrl, data);

        // Assert
        _httpClientFactory.Received(1).CreateClient(LarkWebhookSetting.SettingName);
        Assert.That(_mockHttpMessageHandler.NumberOfCalls, Is.EqualTo(1));
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    public int NumberOfCalls { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        NumberOfCalls++;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }
}