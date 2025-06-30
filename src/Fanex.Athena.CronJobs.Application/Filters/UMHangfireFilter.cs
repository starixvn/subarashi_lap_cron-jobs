namespace Fanex.Athena.CronJobs.Application.Filters;

using System;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using ONEbook.UM.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public class UmHangfireFilterAttribute(IMaintenanceService maintenanceService) : JobFilterAttribute, IClientFilter, IServerFilter
{
    public void OnCreating(CreatingContext filterContext)
    {
        if (maintenanceService.IsUnderMaintenance())
        {
            filterContext.Canceled = true;
        }
    }

    public void OnCreated(CreatedContext filterContext)
    {
        //Nothing to do
    }

    public void OnPerforming(PerformingContext filterContext)
    {
        if (maintenanceService.IsUnderMaintenance())
        {
            filterContext.Canceled = true;
        }
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        //Nothing to do
    }
}