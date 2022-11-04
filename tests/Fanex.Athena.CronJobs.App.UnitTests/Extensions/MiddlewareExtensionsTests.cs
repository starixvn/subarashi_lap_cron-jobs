using Fanex.Athena.CronJobs.App.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Fanex.Athena.CronJobs.App.UnitTests.Extensions
{
    public class MiddlewareExtensionsTests
    {
        private IServiceCollection ServiceCollection { get; set; }
        private IConfiguration Configuration { get; set; }
        private IApplicationBuilder ApplicationBuilder { get; set; }

        [SetUp]
        public void SetUp()
        {
            ServiceCollection = Substitute.For<IServiceCollection>();
            Configuration = Substitute.For<IConfiguration>();
            ApplicationBuilder = Substitute.For<IApplicationBuilder>();
        }

        [Test]
        public void AddUnderMaintenance_WhenCall_NotThrowException()
        {
            //Assert
            Assert.DoesNotThrow(() => MiddlewareExtensions.AddUnderMaintenance(ServiceCollection, Configuration));
        }

        [Test]
        public void AddLogAggregate_WhenCall_NotThrowException()
        {
            //Assert
            Assert.DoesNotThrow(() => MiddlewareExtensions.AddLogAggregate(ServiceCollection));
        }

        [Test]
        public void UsePing_WhenCall_NotThrowExcption()
        {
            //Assert
            Assert.DoesNotThrow(() => MiddlewareExtensions.UsePing(ApplicationBuilder));
        }
    }
}
