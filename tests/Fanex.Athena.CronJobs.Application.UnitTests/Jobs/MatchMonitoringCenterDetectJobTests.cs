namespace Fanex.Athena.CronJobs.Application.UnitTests.Jobs;

using System.Threading.Tasks;
using AutoFixture;
using Fanex.Athena.CronJobs.Application.Jobs;
using Fanex.Athena.CronJobs.Application.Jobs.Manual;
using Fanex.Athena.CronJobs.Domain.Logs;
using Fanex.Athena.CronJobs.Domain.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

public class MatchMonitoringCenterDetectJobTests
{
    private IFixture _fixture;
    private IMatchMonitoringCenterService _matchService;
    private ILogAggregate _logAggregate;
    private IBackgroundJobClient _backgroundJobClient;
    private MatchMonitoringCenterDetectJob _job;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _matchService = Substitute.For<IMatchMonitoringCenterService>();
        _logAggregate = Substitute.For<ILogAggregate>();
        _backgroundJobClient = Substitute.For<IBackgroundJobClient>();
        _backgroundJobClient.Create(Arg.Any<Job>(), Arg.Any<IState>()).Returns("1");
        _job = new MatchMonitoringCenterDetectJob(_matchService, _logAggregate, _backgroundJobClient);
    }

    [Test]
    public async Task ExecuteAsync_ShouldCallDetectAndEnqueueJob()
    {
        // Arrange
        var triggeredData = _fixture.Create<string>();

        _matchService.DetectAlertRulesAsync().Returns(Task.FromResult(triggeredData));

        // Act
        await _job.ExecuteAsync();

        // Assert
        await _matchService.Received(1).DetectAlertRulesAsync();
        _backgroundJobClient.Received(1).Create(
            Arg.Is<Job>(job =>
                job.Type == typeof(LarkWebhookJob) &&
                job.Method.Name == nameof(LarkWebhookJob.Execute) &&
                job.Args.Count == 1 &&
                (string)job.Args[0] == triggeredData),
            Arg.Any<EnqueuedState>());
        _logAggregate.Received().SendLog(LogLevel.Information);
    }
}