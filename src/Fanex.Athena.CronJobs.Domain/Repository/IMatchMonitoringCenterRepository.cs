namespace Fanex.Athena.CronJobs.Domain.Repository;

using System.Threading.Tasks;

public interface IMatchMonitoringCenterRepository
{
    Task<string> DetectAlertRulesAsync();

    Task UpdateMatchAlertAsync(string matchMonitoringData);

    Task UpdateAlertHistoryAsync(string matchMonitoringData);
}