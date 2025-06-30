namespace Fanex.Athena.CronJobs.Domain.ExecuteTasks;

using System.Threading.Tasks;

public interface IJobTask
{
    Task ExecuteAsync();
}