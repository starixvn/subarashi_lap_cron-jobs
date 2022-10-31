using Fanex.Athena.CronJobs.App.Filters;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using NSubstitute;
using NUnit.Framework;
using ONEbook.UM.Services;
using System;

namespace Fanex.Athena.CronJobs.App.UnitTests.Filters
{
    public class UMHangfireFilterAttributeTests
    {
        private IMaintenanceService _maintenanceService { get; set; }

        [SetUp]
        public void SetUp()
        {
            _maintenanceService = Substitute.For<IMaintenanceService>();
        }

        [Test]
        public void OnCreating_WhenNotUnderMaintenance_NotCancled()
        {
            //Arrange
            var context = new CreatingContext(GetCreateContext());
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            context.Canceled = false;
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(false);

            //Action
            filter.OnCreating(context);

            //Assert
            Assert.That(context.Canceled, Is.False);
        }

        [Test]
        public void OnCreating_WhenIsUnderMaintenance_CancledIsTrue()
        {
            //Arrange
            var context = new CreatingContext(GetCreateContext());
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            context.Canceled = false;
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(true);

            //Action
            filter.OnCreating(context);

            //Assert
            Assert.That(context.Canceled, Is.True);
        }

        [Test]
        public void OnCreate_WhenCall_DoNothing()
        {
            //Arrange
            var context = new CreatedContext(GetCreateContext(), null, true, null);
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(true);

            //Assert
            Assert.DoesNotThrow(() => filter.OnCreated(context));
        }

        [Test]
        public void OnPerforming_WhenNotUnderMaintenance_NotCanceled()
        {
            //Arrange
            var context = new PerformingContext(GetPerformContext());
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            context.Canceled = false;
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(false);

            //Action
            filter.OnPerforming(context);

            //Assert
            Assert.That(context.Canceled, Is.False);
        }

        [Test]
        public void OnPerforming_WhenIsUnderMaintenance_CancledIsTrue()
        {
            //Arrange
            var context = new PerformingContext(GetPerformContext());
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            context.Canceled = false;
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(true);

            //Action
            filter.OnPerforming(context);

            //Assert
            Assert.That(context.Canceled, Is.True);
        }

        [Test]
        public void OnPerformed_WhenCall_DoNothing()
        {
            //Arrange
            var context = new PerformedContext(GetPerformContext(), null, true, null);
            var filter = new UMHangfireFilterAttribute(_maintenanceService);
            _maintenanceService.IsUnderMaintenance().ReturnsForAnyArgs(true);

            //Assert
            Assert.DoesNotThrow(() => filter.OnPerformed(context));
        }

        private CreateContext GetCreateContext()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("Test");
            var job = new Job(type, methodInfo);
            return new CreateContext(Substitute.For<JobStorage>(), Substitute.For<IStorageConnection>(), job, Substitute.For<IState>());
        }

        private PerformContext GetPerformContext()
        {
            var type = typeof(TestClass);
            var methodInfo = type.GetMethod("Test");
            var job = new Job(type, methodInfo);
            var backgroundJob = new BackgroundJob(Guid.NewGuid().ToString(), job, DateTime.Now);
            return new PerformContext(null, Substitute.For<IStorageConnection>(), backgroundJob, Substitute.For<IJobCancellationToken>());
        }

        private class TestClass
        {
            public void Test() { }
        }
    }
}
