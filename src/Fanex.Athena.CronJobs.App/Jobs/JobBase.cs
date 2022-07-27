
using Fanex.Athena.CronJobs.Core.ExecuteTasks;
using Hangfire;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    public abstract class JobBase : IJobTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 1)]
        public abstract Task ExecuteAsync();
    }
}
