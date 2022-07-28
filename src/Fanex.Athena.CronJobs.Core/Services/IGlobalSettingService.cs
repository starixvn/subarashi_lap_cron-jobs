using Fanex.Athena.CronJobs.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Services
{
    public interface IGlobalSettingService
    {
        Task<IEnumerable<ServiceSetting>> GetAllSettingAsync(string serviceName);

        Task<string> GetTestIdsAsync(string serviceName);

        string GetTestIds(IEnumerable<ServiceSetting> settings);

        Task<DateTime> GetLastCheckReachThresholdTimeAsync(string serviceName);

        DateTime GetLastCheckReachThresholdTime(IEnumerable<ServiceSetting> settings);

        Task<DateTime> GetLastCheckChangeSettingTimeAsync(string serviceName);

        DateTime GetLastCheckChangeSettingTime(IEnumerable<ServiceSetting> settings);
    }
}
