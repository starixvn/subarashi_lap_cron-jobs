using Fanex.Athena.CronJobs.Core.Constants;
using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Services
{
    public class BellNotificationService : IBellNotificationService
    {
        private IBellNotificationRepository BellNotificationRepository { get; }
        private ILogAggregate LogAggregater { get; }

        private AppSetting AppSetting { get; }

        public BellNotificationService(
            IBellNotificationRepository bellNotificationRepository,
            IOptions<AppSetting> options,
            ILogAggregate logAggregater)
        {
            BellNotificationRepository = bellNotificationRepository;
            LogAggregater = logAggregater;
            AppSetting = options?.Value;
        }

        public async Task InsertBellNotificationAsync(InsertBellNotificationParam param)
        {
            await BellNotificationRepository.InsertNotificationAsync(param);
        }

        public async Task InsertMPABellNotificationAsync(InsertBellNotificationParam param)
        {
            await BellNotificationRepository.InsertMPANotificationAsync(param);
        }

        public async Task<int> GetDWHDailyJobStatusAsync(int jobId)
        {
            return await BellNotificationRepository.GetDWHDailyJobStatusAsync(jobId);
        }

        public async Task<bool> IsAbnormalBetNotificationJobDone()
        {
            var today = GetServerDate().ToString("yyyy-MM-dd");
            var jobDoneDate = await BellNotificationRepository.GetSystemSettingAsync(SystemSettingName.AbnormalBetNotificationJob);

            if (string.IsNullOrEmpty(jobDoneDate.Value) || (DateTime.Parse(today) > DateTime.Parse(jobDoneDate.Value)))
            {
                return false;
            }

            return true;
        }

        public async Task UpdateAbnormalBetNotificationJobDate()
        {
            var today = GetServerDate().ToString("yyyy-MM-dd");
            await BellNotificationRepository.UpdateSystemSettingAsync(new SystemSetting
            {
                Name = SystemSettingName.AbnormalBetNotificationJob,
                Value = today
            });
        }

        public async Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync()
        {
            return await BellNotificationRepository.GetDailyAbnormalBetAsync();
        }

        private DateTime GetServerDate()
        {
            var timezone = TimeZoneInfo.CreateCustomTimeZone("LAP DB Timezone", TimeSpan.FromHours((double)AppSetting.ServerTimeOffSet), "LAP DB Timezone", "LAP DB Timezone");
            var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
            return date;
        }
    }
}