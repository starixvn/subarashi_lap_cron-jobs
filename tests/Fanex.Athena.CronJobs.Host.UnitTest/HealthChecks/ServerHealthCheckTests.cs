namespace Fanex.Athena.CronJobs.Host.UnitTest.HealthChecks;

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Settings;
using AutoFixture;
using FluentAssertions;
using Host.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class ServerHealthCheckTests
{
    private IFixture _fixture;
    private IHttpContextAccessor _httpContextAccessor;
    private IOptions<AppSetting> _appSettingOptions;
    private ServerHealthCheck _healthCheck;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _appSettingOptions = Options.Create(_fixture.Create<AppSetting>());
        _healthCheck = new ServerHealthCheck(_httpContextAccessor, _appSettingOptions);
    }

    [Test]
    public async Task CheckHealthAsync_WhenHttpContextIsNull_ReturnsHealthyWithBasicInfo()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext)null);

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Data.Should().ContainKey("Env");
        result.Data.Should().ContainKey("Host Name");
        result.Data.Should().NotContainKey("Local Ip");
    }

    [Test]
    public async Task CheckHealthAsync_WhenHttpContextExists_ReturnsHealthyWithFullInfo()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Connection.LocalIpAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
        httpContext.Connection.RemoteIpAddress = new IPAddress(new byte[] { 192, 168, 1, 1 });
        _httpContextAccessor.HttpContext.Returns(httpContext);

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Server Info");

        var expectedKeys = new[]
        {
            "Env", "Host Name", "Local Ip", "Local Port", "Client Ip",
            "Server Time", "Last Updated", "Host", "Schema"
        };
        result.Data.Keys.Should().Contain(expectedKeys);
        result.Data["Env"].Should().Be(_appSettingOptions.Value.Environment);
    }
}