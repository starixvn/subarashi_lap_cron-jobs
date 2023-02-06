using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Repository
{
    public interface IBellNotificationRepository
    {
        Task InsertNotificationAsync(InsertBellNotificationParam param);

        Task InsertMPANotificationAsync(InsertBellNotificationParam param);

        Task<int> GetDWHDailyJobStatusAsync(int jobId);

        Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync();

        Task<SystemSetting> GetSystemSettingAsync(string name);

        Task UpdateSystemSettingAsync(SystemSetting setting);
    }
}