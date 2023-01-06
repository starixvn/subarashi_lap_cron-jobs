using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Services
{
    public interface IBellNotificationService
    {
        public Task InsertBellNotificationAsync(InsertBellNotificationParam param);

        public Task<int> GetDWHDailyJobStatusAsync(int jobId);

        public Task<bool> IsAbnormalBetNotificationJobDone();

        public Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync();

        public Task UpdateAbnormalBetNotificationJobDate();
    }
}