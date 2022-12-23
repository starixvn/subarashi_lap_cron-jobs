using Fanex.Athena.CronJobs.App.Filters;
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
        public static IServiceCollection AddCronJobsDI(this IServiceCollection services)
        {
            services.AddRepository()
                .AddService()
                .AddFilter();

            return services;
        }

        public static void UseHangFire(this IApplicationBuilder app, string path)
        {
            app.UseHangfireDashboard(path, new DashboardOptions
            {
                DisplayStorageConnectionString = false,
                Authorization = new[] { new AllowedAnonymouseAuthorizationFilter() },
                IgnoreAntiforgeryToken = true
            });
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IAccumulateRepository, AccumulateRepository>();
            services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IBellNotificationRepository, BellNotificationRepository>();
            return services;
        }

        private static IServiceCollection AddService(this IServiceCollection services)
        {
            services.AddScoped<IGlobalSettingService, GlobalSettingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IBellNotificationService, BellNotificationService>();
            return services;
        }

        private static IServiceCollection AddFilter(this IServiceCollection services)
        {
            services.AddSingleton<GlobalHangfireJobExceptionFilterAttribute>();
            services.AddSingleton<UMHangfireFilterAttribute>();
            return services;
        }
    }
}