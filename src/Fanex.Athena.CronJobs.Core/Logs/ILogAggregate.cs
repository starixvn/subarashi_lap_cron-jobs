using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Logs
{
    public interface ILogAggregate
    {
        Task AppendHeaderAsync(string message);

        Task AppendLogAsync(string message);

        string GetAggregateMsg();

        void SendLog(LogLevel logLevel);
    }
 
}
