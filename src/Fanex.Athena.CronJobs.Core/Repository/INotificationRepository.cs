using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Repository
{
    public interface INotificationRepository
    {
        Task InsertNotificationAsync(InsertNotificationParam param);

        Task<IEnumerable<CustThresholdSetting>> GetCustThresholdSettingAsync(GetSettingThresholdParam param);

        Task<IEnumerable<CustThresholdSetting>> GetCustChangeThresholdSettingAsync(ChangeThresholdSettingParam param);
    }
}
