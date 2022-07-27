using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.ExecuteTasks
{
    public interface IJobTask
    {
        Task ExecuteAsync();
    }
}
