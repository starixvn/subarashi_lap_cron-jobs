using Fanex.Athena.CronJobs.App.Filters;
using Fanex.Athena.CronJobs.App.Jobs;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Services;
using Fanex.Data.Repository;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Fanex.Athena.CronJobs.App.Extensions
{
    public static class JobsHangFireExtensions
    {
        public static IServiceCollection AddHangFireJobs(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddRepository()
                .AddService();
            return services;
        }

        public static void UserHangFire(this IApplicationBuilder app, string path)
        {
            app.UseHangfireDashboard(path, new DashboardOptions
            {
                DisplayStorageConnectionString = true,
                Authorization = new[] { new AllowedAnonymouseAuthorizationFilter() }
            });

            RegisterRecuringTask<NotificateChangeSettingJob>(nameof(NotificateChangeSettingJob), "1/5 * * * *");
            RegisterRecuringTask<NotificateReachThresholdJob>(nameof(NotificateReachThresholdJob), "3/5 * * * *");
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IAccumulateRepository, AccumulateRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            return services;
        }

        private static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IGlobalSettingService, GlobalSettingService>();
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }

        private static void RegisterRecuringTask<T>(string taskName, string cronExpress)
            where T : JobBase
        {
            RecurringJob.AddOrUpdate<T>(taskName, (job) => job.ExecuteAsync(), cronExpress);
        }
    }
}
