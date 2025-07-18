namespace Fanex.Athena.CronJobs.Domain.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;
using Params;

public interface INotificationRepository
{
    Task InsertNotificationAsync(InsertNotificationParam param);

    Task<IEnumerable<CustThresholdSetting>> GetCustThresholdSettingAsync(GetSettingThresholdParam param);

    Task<IEnumerable<CustThresholdSetting>> GetCustChangeThresholdSettingAsync(ChangeThresholdSettingParam param);
}