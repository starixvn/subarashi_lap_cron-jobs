namespace Fanex.Athena.CronJobs.Host.Startup;

using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Application.Filters;
using Application.Jobs;
using Application.Settings;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

public static class WebApplicationSetup
{
    public static void UseHealthChecks(this IApplicationBuilder app, IConfiguration configuration)
    {
        static async Task CustomResponseWriter(HttpContext context, HealthReport report)
        {
            var result = JsonConvert.SerializeObject(
                new
                {
                    status = report.Status.ToString(),
                    monitors = report.Entries.Select(e => new
                        { name = e.Key, status = Enum.GetName(typeof(HealthStatus), e.Value.Status), values = e.Value.Data })
                }, Formatting.Indented);

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        }

        app.UseHealthChecks("/whoami", options: new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResponseWriter = CustomResponseWriter
        });
    }

    public static void UseHangFire(this IApplicationBuilder app, string path, IConfiguration configuration)
    {
        app.UseHangfireDashboard(path, new DashboardOptions
        {
            DisplayStorageConnectionString = false,
            Authorization = [new AllowedAnonymouseAuthorizationFilter()],
            IgnoreAntiforgeryToken = true
        });

        // add Cronjob
        var appSetting = configuration.GetSection(AppSetting.SettingName).Get<AppSetting>();

        if (appSetting.IsMainService)
        {
            RegisterRecurringTask<InsertAbnormalBetNotificationJob>(nameof(InsertAbnormalBetNotificationJob), "* */15 * * * *");
            RegisterRecurringTask<MatchMonitoringCenterDetectJob>(nameof(MatchMonitoringCenterDetectJob), "1/5 * * * * *");
        }

        RegisterRecurringTask<NotificateChangeSettingJob>(nameof(NotificateChangeSettingJob), "1/5 * * * * *");
        RegisterRecurringTask<NotificateReachThresholdJob>(nameof(NotificateReachThresholdJob), "1/30 * * * * *");
    }

    private static void RegisterRecurringTask<T>(string taskName, string cronExpression)
        where T : JobBase
    {
        RecurringJob.AddOrUpdate<T>(taskName, job => job.ExecuteAsync(), cronExpression);
    }
}