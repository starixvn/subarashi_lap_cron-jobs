using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Services
{
    public interface IMatchMonitoringCenterService
    {
        Task DetectAlertRulesAsync();
    }
}