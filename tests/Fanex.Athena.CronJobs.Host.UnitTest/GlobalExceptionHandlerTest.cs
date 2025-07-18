namespace Fanex.Athena.CronJobs.Host.UnitTest;

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;

public class GlobalExceptionHandlerTests
{
    private IFixture _fixture;
    private ILogger _logger;
    private GlobalExceptionHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _logger = Substitute.For<ILogger>();
        _handler = new GlobalExceptionHandler(_logger);
    }

    [Test]
    public async Task TryHandleAsync_ShouldHandleExceptionAndReturnCorrectResponse()
    {
        // Arrange
        var exception = _fixture.Create<Exception>();
        var httpContext = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        var expectedMsg = JsonConvert.SerializeObject(new
        {
            code = 500,
            type = "",
            message = "An internal server error occurred while a request was processed."
        });

        // Act
        var result = await _handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _logger.Received(1).CriticalAsync(Arg.Any<string>(), exception);

        httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        httpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var bodyContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        bodyContent.Should().Be(expectedMsg);
    }
}