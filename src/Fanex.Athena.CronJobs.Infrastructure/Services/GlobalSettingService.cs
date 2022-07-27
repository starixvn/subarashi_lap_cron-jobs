using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Services
{
    public class GlobalSettingService : IGlobalSettingService
    {
        private const string TEST_USER_SETTING_NAME = "SuperUUS";
        private const string LAST_CHECK_TIME_SETTING_NAME = "LastCheckTime";
        private const string LAST_SCAN_SETTING_DATE = "LastScanSettingDate";

        private IGlobalSettingRepository SettingRepository { get; }

        public GlobalSettingService(IGlobalSettingRepository settingRepository)
        {
            SettingRepository = settingRepository;
        }

        public Task<IEnumerable<ServiceSetting>> GetAllSettingAsync(string serviceName)
            => SettingRepository.GetSetting(serviceName);

        public DateTime GetLastCheckReachThresholdTime(IEnumerable<ServiceSetting> settings)
        {
            var setting = settings.FirstOrDefault(_ => string.Equals(_.SettingName, LAST_CHECK_TIME_SETTING_NAME, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(setting.SettingValue))
            {
                throw new InvalidCastException("Null or empty setting value");
            }
            return Convert.ToDateTime(setting.SettingValue);
        }

        public DateTime GetLastCheckChangeSettingTime(IEnumerable<ServiceSetting> settings)
        {
            var setting = settings.FirstOrDefault(_ => string.Equals(_.SettingName, LAST_SCAN_SETTING_DATE, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(setting.SettingValue))
            {
                throw new InvalidCastException("Null or empty setting value");
            }
            return Convert.ToDateTime(setting.SettingValue);
        }

        public async Task<DateTime> GetLastCheckChangeSettingTimeAsync(string serviceName)
        {
            var settings = await GetAllSettingAsync(serviceName);
            return GetLastCheckChangeSettingTime(settings);
        }

        public async Task<DateTime> GetLastCheckReachThresholdTimeAsync(string serviceName)
        {
            var settings = await GetAllSettingAsync(serviceName);
            return GetLastCheckReachThresholdTime(settings);
        }

        public string GetTestIds(IEnumerable<ServiceSetting> settings)
        {
            var setting = settings.FirstOrDefault(_ => string.Equals(_.SettingName, TEST_USER_SETTING_NAME, StringComparison.OrdinalIgnoreCase));
            return setting?.SettingValue;
        }

        public async Task<string> GetTestIdsAsync(string serviceName)
        {
            var settings = await GetAllSettingAsync(serviceName);
            return GetTestIds(settings);
        }
    }
}
