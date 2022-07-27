using Fanex.Athena.CronJobs.Core.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Repository
{
    public interface IGlobalSettingRepository
    {
        Task<IEnumerable<ServiceSetting>> GetSetting(string serviceName);
    }
}
