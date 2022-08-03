using Fanex.Athena.CronJobs.App.Extensions;
using Fanex.Athena.CronJobs.App.Filters;
using Fanex.Athena.CronJobs.App.Settings;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.Athena.CronJobs
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
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
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

            var redisConfig = Configuration.GetSection("Redis").Get<RedisSetting>();
            var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                KeepAlive = 180,
                EndPoints = { { redisConfig.Server, redisConfig.Port } },
                Password = redisConfig.Pass,
                DefaultDatabase = redisConfig.Database,
            });

            services.AddCronJobs();

            services.AddHangfire((serviceProvider, configuration) =>
            {
                configuration.UseRedisStorage(redis, new Hangfire.Redis.RedisStorageOptions
                {
                    Prefix = redisConfig.AppName,
                    Db = redisConfig.Database
                });
                configuration.UseFilter(serviceProvider.GetService<GlobalHangfireJobExceptionFilterAttribute>());
                configuration.UseFilter(serviceProvider.GetService<UMHangfireFilterAttribute>());
            });

            services.AddHangfireServer();
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
    }
}
