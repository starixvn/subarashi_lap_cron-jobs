using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using ONEbook.UM.Services;

namespace Fanex.Athena.CronJobs.App.Filters
{
    public class UMHangfireFilterAttribute : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public IMaintenanceService MaintenanceService { get; }

        public UMHangfireFilterAttribute(IMaintenanceService maintenanceService)
        {
            MaintenanceService = maintenanceService;
        }

        public void OnCreating(CreatingContext filterContext)
        {
            if (MaintenanceService.IsUnderMaintenance())
            {
                filterContext.Canceled = true;
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            if (MaintenanceService.IsUnderMaintenance())
            {
                filterContext.Canceled = true;
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
}
