namespace Fanex.Athena.CronJobs.Application.Filters;

using Hangfire.Dashboard;

public class AllowedAnonymouseAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}