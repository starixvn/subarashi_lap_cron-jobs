namespace Fanex.Athena.CronJobs.Domain.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;
using Params;

public interface IBellNotificationRepository
{
    Task InsertNotificationAsync(InsertBellNotificationParam param);

    Task InsertMPANotificationAsync(InsertBellNotificationParam param);

    Task<int> GetDWHDailyJobStatusAsync(int jobId);

    Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync();

    Task<SystemSetting> GetSystemSettingAsync(string name);

    Task UpdateSystemSettingAsync(SystemSetting setting);
}