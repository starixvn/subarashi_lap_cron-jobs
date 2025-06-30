namespace Fanex.Athena.CronJobs.Infrastructure.Logs;

using System;
using System.Text;
using Domain.Logs;
using Microsoft.Extensions.Logging;

public class LogAggregate(ILogger<LogAggregate> logger) : ILogAggregate
{
    private StringBuilder builder = new();

    public void AppendLog(string message)
        => builder.AppendLine($"{DateTime.Now}: {message}");


    public string GetAggregateMsg()
    {
        var logmsg = builder.ToString();
        return logmsg;
    }

    public void SendLog(LogLevel logLevel)
    {
        if (builder.Length == 0)
        {
            return;
        }

        logger.Log(logLevel, builder.ToString());
        builder = new StringBuilder();
    }

    public void AppendHeader(string message)
        => builder.AppendLine($"{message.ToUpperInvariant()}");
}