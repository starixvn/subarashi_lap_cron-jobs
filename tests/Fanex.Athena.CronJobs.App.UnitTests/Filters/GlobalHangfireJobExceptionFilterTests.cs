using Fanex.Athena.CronJobs.App.Filters;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.UnitTests.Filters
{
    public class GlobalHangfireJobExceptionFilterTests
    {
        private ILogger<GlobalHangfireJobExceptionFilterAttribute> Logger { get; set; }
        private ElectStateContext ElectStateContext { get; set; }

        [SetUp]
        public void SetUp()
        {
            Logger = Substitute.For<ILogger<GlobalHangfireJobExceptionFilterAttribute>>();
            ElectStateContext = new ElectStateContext(GetApplyStateContext());
        }

        [Test]
        public void OnStateElection_WhenCallWithNotFailState_NotCallLogger()
        {
            //Arrange
            ElectStateContext.CandidateState = new DeletedState();
            var filter = new GlobalHangfireJobExceptionFilterAttribute(Logger);

            //Action
            filter.OnStateElection(ElectStateContext);

            //Assert
            Assert.That(Logger.ReceivedCalls()
                    .Select(call => call.GetArguments())
                    .Count(callArguments => ((LogLevel)callArguments[0]).Equals(LogLevel.Error)),
                Is.EqualTo(0));
        }

        [Test]
        public void OnStateElection_WhenCallWithFailState_CallLogger()
        {
            //Arrange
            ElectStateContext.CandidateState = new FailedState(new Exception("Test"));
            var filter = new GlobalHangfireJobExceptionFilterAttribute(Logger);

            //Action
            filter.OnStateElection(ElectStateContext);

            //Assert
            Assert.That(Logger.ReceivedCalls()
                    .Select(call => call.GetArguments())
                    .Count(callArguments => ((LogLevel)callArguments[0]).Equals(LogLevel.Error)),
                Is.EqualTo(1));
        }

        private static ApplyStateContext GetApplyStateContext()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("Test");
            var job = new Job(type, methodInfo);
            var backgroundJob = new BackgroundJob(Guid.NewGuid().ToString(), job, DateTime.Now);
            return new ApplyStateContext(Substitute.For<JobStorage>(), Substitute.For<IStorageConnection>(), Substitute.For<IWriteOnlyTransaction>(), backgroundJob, Substitute.For<IState>(), "");
        }

        private class TestClass
        {
            public void Test() { }
        }
    }
}
