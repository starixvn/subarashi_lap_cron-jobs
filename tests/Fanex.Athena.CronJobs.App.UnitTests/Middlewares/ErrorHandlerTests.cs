using Fanex.Athena.CronJobs.App.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Fanex.Athena.CronJobs.App.UnitTests.Middlewares
{
    public class ErrorHandlerTests
    {
        private RequestDelegate Next { get; set; }
        private ILogger<ErrorHandler> Logger { get; set; }

        [SetUp]
        public void SetUp()
        {
            Next = Substitute.For<RequestDelegate>();
            Logger = Substitute.For<ILogger<ErrorHandler>>();
        }

        [Test]
        public async Task Invoke_WhenCallWithNoException_NotLogging()
        {
            //Arrange
            var errorHandler = new ErrorHandler(Next, Logger);

            //Action
            await errorHandler.Invoke(default);

            // Assert
            Assert.That(Logger.ReceivedCalls()
                    .Select(call => call.GetArguments())
                    .Count(callArguments => ((LogLevel)callArguments[0]).Equals(LogLevel.Critical)),
                Is.EqualTo(0));
        }

        [Test]
        public async Task Invoke_WhenCallAndThrowException_ErrorHandlerHandleException()
        {
            //Arrange
            var context = Substitute.For<HttpContext>();
            var reponse = Substitute.For<HttpResponse>();
            var feature = Substitute.For<IFeatureCollection>();
            var stream = new MemoryStream();

            context.Response.ReturnsForAnyArgs(reponse);
            context.Features.ReturnsForAnyArgs(feature);

            reponse.HttpContext.ReturnsForAnyArgs(context);
            reponse.Headers.ReturnsForAnyArgs(new HeaderDictionary());
            reponse.Body = stream;

            feature.Get<IHttpResponseFeature>().ReturnsForAnyArgs(new HttpResponseFeature());

            Next.Invoke(context).ReturnsForAnyArgs(x => throw new Exception("Test"));
            var errorHandler = new ErrorHandler(Next, Logger);

            //Action
            await errorHandler.Invoke(context);

            //Assert
            Assert.That(context.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
            stream.Dispose();
        }
    }
}
