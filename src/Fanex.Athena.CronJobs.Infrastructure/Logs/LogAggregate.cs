namespace Fanex.Athena.CronJobs.Infrastructure.Logs;

using System;
using System.Text;
using Domain.Logs;
using Microsoft.Extensions.Logging;
using ILogger = Logging.ILogger;

public class LogAggregate(ILogger logger) : ILogAggregate
{
    private StringBuilder builder = new();

    public void AppendLog(string message)
        => builder.AppendLine($"{DateTimeOffset.Now}: {message}");


    public string GetAggregateMsg()
    {
        var logMsg = builder.ToString();
        return logMsg;
    }

    public void SendLog(LogLevel logLevel)
    {
        if (builder.Length == 0)
        {
            return;
        }

        var message = builder.ToString();
        switch (logLevel)
        {
            case LogLevel.Information:
                logger.Info(message);
                break;
            case LogLevel.Warning:
                logger.Warn(message);
                break;
            case LogLevel.Error:
                logger.Error(message);
                break;
            case LogLevel.Critical:
                logger.Critical(message);
                break;
            default:
                logger.Debug(message);
                break;
        }

        builder = new StringBuilder();
    }

    public void AppendHeader(string message)
        => builder.AppendLine($"{message.ToUpperInvariant()}");
}