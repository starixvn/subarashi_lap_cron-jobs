using Hangfire.Dashboard;

namespace Fanex.Athena.CronJobs.App.Filters
{
    public class AllowedAnonymouseAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
