namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Logs;

using Infrastructure.Logs;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using ILogger = Logging.ILogger;

public class LogAggregateTests
{
    private ILogger Logger { get; set; }

    [SetUp]
    public void SetUp()
    {
        Logger = Substitute.For<ILogger>();
    }

    [Test]
    public void AppendLog_WhenCall_LogMessageAppend()
    {
        // Arrange
        var logAggregate = new LogAggregate(Logger);
        var msg = "test1";

        // Action
        logAggregate.AppendLog(msg);

        // Assert
        var arreggateMsg = logAggregate.GetAggregateMsg();
        Assert.That(arreggateMsg, Does.Contain(msg));
    }

    [Test]
    public void AppendHeader_WhenCall_LogMessageAppend()
    {
        // Arrange
        var logAggregate = new LogAggregate(Logger);
        var msg = "test1";

        // Action
        logAggregate.AppendHeader(msg);

        // Assert
        var arreggateMsg = logAggregate.GetAggregateMsg();
        Assert.That(arreggateMsg.Trim(), Is.EqualTo(msg.ToUpperInvariant()));
    }

    [Test]
    public void SendLog_WhenCallWithNoMsg_NotCallLogger()
    {
        // Arrange
        var logAggregate = new LogAggregate(Logger);

        // Action
        logAggregate.SendLog(LogLevel.Information);

        // Assert
        Assert.That(Logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Count(callArguments => ((LogLevel)callArguments[0]).Equals(LogLevel.Information)),
            Is.EqualTo(0));
    }

    [Test]
    public void SendLog_WhenCallWithMsg_CallLogger()
    {
        // Arrange
        var logAggregate = new LogAggregate(Logger);
        var msg = "test".ToUpperInvariant();
        logAggregate.AppendHeader(msg);

        // Action
        logAggregate.SendLog(LogLevel.Information);

        // Assert
        Assert.That(Logger.ReceivedCalls()
                .Select(call => call.GetArguments())
                .Count(callArguments => callArguments[0].ToString().Contains(msg)),
            Is.EqualTo(1));
    }
}