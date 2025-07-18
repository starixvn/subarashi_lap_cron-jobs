namespace Fanex.Athena.CronJobs.Host.UnitTest.HealthChecks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Data.Vault;
using FluentAssertions;
using Host.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

public class VaultHealthCheckTests
{
    private IFixture _fixture;
    private VaultDbAliasService _dbAliasService;
    private VaultHealthCheck _healthCheck;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _dbAliasService = Substitute.For<VaultDbAliasService>();
    }

    private void BuildHealthCheck(IDictionary<string, string> connectionStrings)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(connectionStrings)
            .Build();
        _healthCheck = new VaultHealthCheck(configuration, _dbAliasService);
    }

    [Test]
    public async Task CheckHealthAsync_WithValidConnections_ReturnsHealthy()
    {
        // Arrange
        var rawConnections = new Dictionary<string, string>
        {
            { "Db1.ReadOnly", "conn1_value" },
            { "Db2.Write", "conn2_value" },
            { "Db1.Write", "conn3_value" }
        };

        var formattedConnections = rawConnections.ToDictionary(kvp => $"ConnectionStrings:{kvp.Key}", kvp => kvp.Value);
        BuildHealthCheck(formattedConnections);

        var expectedAliases = new Dictionary<string, object>
        {
            { "Db1", _fixture.Create<string>() },
            { "Db2", _fixture.Create<string>() }
        };

        foreach (var alias in expectedAliases)
        {
            _dbAliasService.GetIp(alias.Key).Returns((string)alias.Value);
        }

        // Act
        var result = await _healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("Vault Info");
        result.Data.Should().BeEquivalentTo(expectedAliases);
    }

    [Test]
    public async Task CheckHealthAsync_WhenDbAliasServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var connections = new Dictionary<string, string>
        {
            { "ConnectionStrings:Db1", "conn1_value" }
        };
        BuildHealthCheck(connections);
        var exception = _fixture.Create<Exception>();
        _dbAliasService.GetIp(Arg.Any<string>()).Throws(exception);

        // Act
        Func<Task> action = () => _healthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        await action.Should().ThrowAsync<Exception>().WithMessage(exception.Message);
    }
}