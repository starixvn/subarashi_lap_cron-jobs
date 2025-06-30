namespace Fanex.Athena.CronJobs.Host.UnitTest;

using System.Net;
using FluentAssertions;
using Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NSubstitute;

public class GlobalExceptionHandlerTest
{
    private readonly GlobalExceptionHandler handler;
    private readonly ILogger logger;

    public GlobalExceptionHandlerTest()
    {
        logger = Substitute.For<ILogger>();
        handler = new GlobalExceptionHandler(logger);
    }

    [Test]
    public async Task GlobalExceptionHandler_ShouldHandleExceptions_ReturnCorrectData()
    {
        // Arrange
        var exception = new Exception("Test exception");
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
        var result = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        // Assert
        await logger.Received().CriticalAsync(Arg.Is<string>(msg => msg.Contains("Test exception")), exception);
        result.Should().BeTrue();
        httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        httpContext.Response.ContentType.Should().Be("application/json; charset=utf-8");
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var bodyContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        bodyContent.Should().Be(expectedMsg);
    }
}