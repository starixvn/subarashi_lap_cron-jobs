namespace Fanex.Athena.CronJobs.Host;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Helpers;
using Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

public sealed class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var errorMsg = ErrorMessageHelper.GenerateMessage(exception, httpContext);
        await logger.CriticalAsync(errorMsg, exception);

        httpContext.Response.Clear();
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/json";

        var result = new
        {
            code = 500,
            type = "",
            message = "An internal server error occurred while a request was processed."
        };

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);
        return true;
    }
}