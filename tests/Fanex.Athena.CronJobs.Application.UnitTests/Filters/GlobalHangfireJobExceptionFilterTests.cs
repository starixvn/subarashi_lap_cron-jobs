namespace Fanex.Athena.CronJobs.Application.UnitTests.Filters;

using Application.Filters;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using NSubstitute;
using NUnit.Framework;
using ILogger = Logging.ILogger;

public class GlobalHangfireJobExceptionFilterTests
{
    private ElectStateContext _electStateContext;
    private ILogger _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = Substitute.For<ILogger>();
        _electStateContext = new ElectStateContext(GetApplyStateContext());
    }

    [Test]
    public void OnStateElection_WhenCallWithNotFailState_NotCallLogger()
    {
        //Arrange
        _electStateContext.CandidateState = new DeletedState();
        var filter = new GlobalHangfireJobExceptionFilterAttribute(_logger);

        //Action
        filter.OnStateElection(_electStateContext);

        //Assert
        _logger.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<Exception>());
    }

    [Test]
    public void OnStateElection_WhenCallWithFailState_CallLogger()
    {
        //Arrange
        _electStateContext.CandidateState = new FailedState(new Exception("Test"));
        var filter = new GlobalHangfireJobExceptionFilterAttribute(_logger);

        //Action
        filter.OnStateElection(_electStateContext);

        //Assert
        _logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>());
    }

    private static ApplyStateContext GetApplyStateContext()
    {
        var type = typeof(TestClass);
        var methodInfo = type.GetMethod("Test");
        var job = new Job(type, methodInfo);
        var backgroundJob = new BackgroundJob(Guid.NewGuid().ToString(), job, DateTime.Now);
        var state = new FailedState(new Exception());
        return new ApplyStateContext(Substitute.For<JobStorage>(), Substitute.For<IStorageConnection>(), Substitute.For<IWriteOnlyTransaction>(), backgroundJob,
            state, "");
    }

    private class TestClass
    {
        public void Test()
        {
        }
    }
}