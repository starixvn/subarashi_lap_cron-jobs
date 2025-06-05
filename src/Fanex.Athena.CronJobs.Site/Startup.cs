using Fanex.Athena.CronJobs.App.Extensions;
using Fanex.Athena.CronJobs.App.Filters;
using Fanex.Athena.CronJobs.App.Jobs;
using Fanex.Athena.CronJobs.App.Middlewares;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Athena.CronJobs.Site
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigFanexData();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddSiteHealthChecks(Configuration);
            services.AddUnderMaintenance(Configuration);
            services.AddLogAggregate();

            AddHangFireService(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePing();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseMiddleware<ErrorHandler>();
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            AddRecurringJobs();
            app.UseHangFire("/jobs");
            app.UseSiteHealthChecks(Configuration);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }

        private void AddHangFireService(IServiceCollection services)
        {
            services.Configure<NotificationSetting>(Configuration.GetSection("NotificationSetting"));
            services.Configure<BellNotificationSetting>(Configuration.GetSection("BellNotificationSetting"));
            services.Configure<AppSetting>(Configuration.GetSection("App"));

            var redisConfig = Configuration.GetSection("Redis").Get<RedisSetting>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                KeepAlive = 180,
                EndPoints = { { redisConfig.Server, redisConfig.Port } },
                Password = redisConfig.Pass,
                DefaultDatabase = redisConfig.Database,
            });

            services.AddCronJobsDI();

            services.AddHangfire((serviceProvider, configuration) =>
            {
                configuration.UseRedisStorage(redis, new Hangfire.Redis.RedisStorageOptions
                {
                    Prefix = redisConfig.AppName,
                    Db = redisConfig.Database
                }).WithJobExpirationTimeout(TimeSpan.FromHours(2));
                configuration.UseFilter(serviceProvider.GetService<GlobalHangfireJobExceptionFilterAttribute>());
                configuration.UseFilter(serviceProvider.GetService<UMHangfireFilterAttribute>());
            });

            services.AddHangfireServer(options =>
            {
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });
        }

        private void ConfigFanexData()
        {
            var listConnectionName = Configuration.GetSection("ConnectionStrings").GetChildren().Select(_ => _.Key);
            var connectionStrings = new Dictionary<string, ConnectionConfiguration>(listConnectionName
                .Select(connectionName => new KeyValuePair<string, ConnectionConfiguration>(connectionName,
                    new ConnectionConfiguration(connectionName, Configuration.GetConnectionString(connectionName)))
                ));

            DbSettingProviderManager
                .StartNewSession()
                .WithSqlServer(resourcePath: "~/SpConfig/FanexSpName.xml")
                .Use(connectionStrings)
                .Run();
        }

        private void AddRecurringJobs()
        {
            if (Configuration != null && bool.Parse(Configuration.GetSection("App:IsMainService").Value))
            {
                RegisterRecuringTask<InsertAbnormalBetNotificationJob>(nameof(InsertAbnormalBetNotificationJob),
                    "* */15 * * * *");
                RegisterRecuringTask<MatchMonitoringCenterDetectJob>(nameof(MatchMonitoringCenterDetectJob),
                    "1/5 * * * * *");
            }
            RegisterRecuringTask<NotificateChangeSettingJob>(nameof(NotificateChangeSettingJob), "1/5 * * * * *");
            RegisterRecuringTask<NotificateReachThresholdJob>(nameof(NotificateReachThresholdJob), "1/30 * * * * *");
        }

        private void RegisterRecuringTask<T>(string taskName, string cronExpress)
            where T : JobBase
        {
            RecurringJob.AddOrUpdate<T>(taskName, (job) => job.ExecuteAsync(), cronExpress);
        }
    }
}