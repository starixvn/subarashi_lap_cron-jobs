using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Repository
{
    public class BellNotificationRepository : IBellNotificationRepository
    {
        private IDynamicRepository DynamicRepository { get; }

        public BellNotificationRepository(IDynamicRepository dynamicRepository)
            => DynamicRepository = dynamicRepository;

        public Task InsertNotificationAsync(InsertBellNotificationParam param)
        {
            return DynamicRepository.ExecuteAsync(new BellNotificationCommand
            {
                MessageType = param.MessageType,
                LicenseeNames = param.LicenseeNames,
                SubjectEn = param.SubjectEn,
                SubjectCs = param.SubjectCs,
                ContentEn = param.ContentEn,
                ContentCs = param.ContentCs,
                EventDate = param.EventDate
            });
        }

        public Task<int> GetDWHDailyJobStatusAsync(int jobId)
        {
            return DynamicRepository.GetAsync<int>(new DWHDailyJobStatusCriteria
            {
                TaskIDList = jobId.ToString(),
            });
        }

        public Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync()
        {
            return DynamicRepository.FetchAsync<AbnormalBet>(new GetAbnormalBetCriteria { });
        }

        public Task<SystemSetting> GetSystemSettingAsync(string name)
        {
            return DynamicRepository.GetAsync<SystemSetting>(new GetSystemSettingCriteria
            {
                Name = name
            });
        }

        public Task UpdateSystemSettingAsync(SystemSetting setting)
        {
            return DynamicRepository.ExecuteAsync(new SystemSettingCommand
            {
                Name = setting.Name,
                Value = setting.Value
            });
        }
    }
}