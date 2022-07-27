using Fanex.Athena.CronJobs.Core.Params;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Services
{
    public interface INotificationService
    {
        Task NotificateReachThresholdAsync(NotificationServiceParam param);

        Task NotificateChangeSettingAsync(NotificationServiceParam param);
    }
}
