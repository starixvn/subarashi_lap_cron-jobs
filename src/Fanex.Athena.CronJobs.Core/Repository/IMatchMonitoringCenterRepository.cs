using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Repository
{
    public interface IMatchMonitoringCenterRepository
    {
        Task<string> DetectAlertRulesAsync();

        Task UpdateMatchAlertAsync(string matchMonitoringData);

        Task UpdateAlertHistoryAsync(string matchMonitoringData);
    }
}