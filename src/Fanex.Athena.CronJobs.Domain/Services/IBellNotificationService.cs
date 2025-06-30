namespace Fanex.Athena.CronJobs.Domain.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;
using Params;

public interface IBellNotificationService
{
    public Task InsertBellNotificationAsync(InsertBellNotificationParam param);

    public Task InsertMPABellNotificationAsync(InsertBellNotificationParam param);

    public Task<int> GetDWHDailyJobStatusAsync(int jobId);

    public Task<bool> IsAbnormalBetNotificationJobDone();

    public Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync();

    public Task UpdateAbnormalBetNotificationJobDate();
}