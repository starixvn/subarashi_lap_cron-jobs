using Fanex.Athena.CronJobs.Core.Logs;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Logs
{
    public class LogAggregate : ILogAggregate
    {
        private readonly ILogger _logger;
        private StringBuilder Builder { get; set; }

        public LogAggregate(ILogger<LogAggregate> logger)
        {
            Builder = new StringBuilder();
            _logger = logger;
        }

        public Task AppendLogAsync(string message)
        {
            Builder.AppendLine($"{DateTime.Now}: {message}");
            return Task.CompletedTask;
        }

        public string GetAggregateMsg()
        {
            var logmsg = Builder.ToString();
            return logmsg;
        }

        public void SendLogAsync(LogLevel logLevel)
        {
            if (Builder.Length == 0)
            {
                return;
            }
            _logger.Log(logLevel, Builder.ToString());
        }
    }
}
