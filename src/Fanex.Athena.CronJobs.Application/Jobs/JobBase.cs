namespace Fanex.Athena.CronJobs.Application.Jobs;

using System.Threading.Tasks;
using Domain.ExecuteTasks;
using Domain.Logs;
using Hangfire;
using Microsoft.Extensions.Logging;

public abstract class JobBase(ILogAggregate logAggregate) : IJobTask
{
    [DisableConcurrentExecution(60)]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
    public async Task ExecuteAsync()
    {
        try
        {
            await DoJobAsync();
        }
        finally
        {
            logAggregate.SendLog(LogLevel.Information);
        }
    }

    protected abstract Task DoJobAsync();
}