namespace Fanex.Athena.CronJobs.Domain.Logs;

using Microsoft.Extensions.Logging;

public interface ILogAggregate
{
    void AppendHeader(string message);

    void AppendLog(string message);

    string GetAggregateMsg();

    void SendLog(LogLevel logLevel);
}