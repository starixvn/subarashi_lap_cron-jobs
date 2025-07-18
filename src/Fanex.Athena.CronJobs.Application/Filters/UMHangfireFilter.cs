namespace Fanex.Athena.CronJobs.Application.Filters;

using System;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using ONEbook.UM.Services;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public class UmHangfireFilterAttribute(IMaintenanceService maintenanceService) : JobFilterAttribute, IClientFilter, IServerFilter
{
    public void OnCreating(CreatingContext context)
    {
        if (maintenanceService.IsUnderMaintenance())
        {
            context.Canceled = true;
        }
    }

    public void OnCreated(CreatedContext context)
    {
        //Nothing to do
    }

    public void OnPerforming(PerformingContext context)
    {
        if (maintenanceService.IsUnderMaintenance())
        {
            context.Canceled = true;
        }
    }

    public void OnPerformed(PerformedContext context)
    {
        //Nothing to do
    }
}