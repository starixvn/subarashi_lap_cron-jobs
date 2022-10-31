using Fanex.Athena.CronJobs.App.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.UnitTests.Extensions
{
    public class JobsHangFireExtensionsTests
    {
        private IServiceCollection ServiceCollection { get; set; }
        private IApplicationBuilder ApplicationBuilder { get; set; }


        [SetUp]
        public void SetUp()
        {
            ServiceCollection = Substitute.For<IServiceCollection>();
            ApplicationBuilder = Substitute.For<IApplicationBuilder>();
        }

        [Test]
        public void AddCronJobs_WhenCall_NotThrowException()
        {
            //Assert
            Assert.DoesNotThrow(() => JobsHangFireExtensions.AddCronJobs(ServiceCollection));
        }
    }
}
