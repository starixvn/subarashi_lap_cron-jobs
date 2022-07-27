using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Repository
{
    public class GlobalSettingRepository : IGlobalSettingRepository
    {
        public IDynamicRepository Repository { get; }

        public GlobalSettingRepository(IDynamicRepository repository)
        {
            Repository = repository;
        }

        public Task<IEnumerable<ServiceSetting>> GetSetting(string serviceName)
            => Repository.FetchAsync<ServiceSetting>(new GlobalSettingCriteria { ServiceName = serviceName });
    }
}
