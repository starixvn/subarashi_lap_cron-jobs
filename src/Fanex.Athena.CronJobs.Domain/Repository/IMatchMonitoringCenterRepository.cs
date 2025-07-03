namespace Fanex.Athena.CronJobs.Domain.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;

public interface IMatchMonitoringCenterRepository
{
    Task<string> DetectAlertRulesAsync();

    Task UpdateMatchAlertAsync(string matchMonitoringData);

    Task UpdateAlertHistoryAsync(string matchMonitoringData);

    Task<IEnumerable<TriggeredDataSendWebHook>> GetTriggeredDataSendWebHookAsync(string licenseeName, string matchMonitoringData);
}