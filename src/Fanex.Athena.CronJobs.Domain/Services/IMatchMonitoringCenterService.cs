namespace Fanex.Athena.CronJobs.Domain.Services;

using System.Threading.Tasks;

public interface IMatchMonitoringCenterService
{
    Task DetectAlertRulesAsync();
}