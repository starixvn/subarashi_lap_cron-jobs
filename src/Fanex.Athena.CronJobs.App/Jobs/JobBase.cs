
using Fanex.Athena.CronJobs.Core.ExecuteTasks;
using Fanex.Athena.CronJobs.Core.Logs;
using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "Wrapper class for hangfire")]
    public abstract class JobBase : IJobTask
    {
        private readonly ILogAggregate _logAggregate;

        protected JobBase(ILogAggregate logAggregate)
            => _logAggregate = logAggregate;

        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public async Task ExecuteAsync()
        {
            try
            {
                await DoJobAsync();
            }
            catch (System.Exception)
            {
                _logAggregate.SendLog(LogLevel.Information);
                throw;
            }
            _logAggregate.SendLog(LogLevel.Information);
        }

        protected abstract Task DoJobAsync();
    }
}
