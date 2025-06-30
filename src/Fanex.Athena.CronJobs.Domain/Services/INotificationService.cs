namespace Fanex.Athena.CronJobs.Domain.Services;

using System.Threading.Tasks;
using Params;

public interface INotificationService
{
    Task NotificateReachThresholdAsync(NotificationServiceParam param);

    Task NotificateChangeSettingAsync(NotificationServiceParam param);
}