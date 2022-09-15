
using Fanex.Athena.CronJobs.Core.ExecuteTasks;
using Hangfire;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Jobs
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "Wrapper class for hangfire")]
    public abstract class JobBase : IJobTask
    {
        [DisableConcurrentExecution(60)]
        [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Fail, Attempts = 1)]
        public abstract Task ExecuteAsync();
    }
}
