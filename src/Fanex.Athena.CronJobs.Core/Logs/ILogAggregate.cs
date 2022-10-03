using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Logs
{
    public interface ILogAggregate
    {
        Task AppendLogAsync(string message);

        string GetAggregateMsg();

        void SendLogAsync(LogLevel logLevel);
    }
 
}
