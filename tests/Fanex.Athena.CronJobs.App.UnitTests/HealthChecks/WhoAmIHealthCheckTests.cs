using Fanex.Athena.CronJobs.App.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.UnitTests.HealthChecks
{
    public class WhoAmIHealthCheckTests
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private IWebHostEnvironment WebHostEnvironment { get; set; }

        [SetUp]
        public void SetUp()
        {
            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            WebHostEnvironment = Substitute.For<IWebHostEnvironment>();
        }

        [Test]
        public async Task CheckHealthAsync_WhenCall_ReturnHealthy()
        {
            //Arrange
            var healthCheck = new WhoAmIHealthCheck(HttpContextAccessor, WebHostEnvironment);
            HttpContextAccessor.HttpContext.Connection.LocalIpAddress.Returns(new System.Net.IPAddress(100));
            HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.Returns(new System.Net.IPAddress(100));


            //Action
            var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());

            //Assert
            Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
        }
    }
}
