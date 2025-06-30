namespace Fanex.Athena.CronJobs.Domain.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;

public interface IGlobalSettingRepository
{
    Task<IEnumerable<ServiceSetting>> GetSetting(string serviceName);
}