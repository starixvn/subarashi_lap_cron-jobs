namespace Fanex.Athena.CronJobs.Host.Configurations;

using System;
using Application.Filters;
using Application.Settings;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

public class HangFireServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(RedisSetting.SettingName).Get<RedisSetting>();

        var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            KeepAlive = 180,
            EndPoints = { { redisSetting.Server, redisSetting.Port } },
            Password = redisSetting.Pass,
            DefaultDatabase = redisSetting.Database
        });

        services.AddHangfire((serviceProvider, hangfireConfiguration) =>
        {
            hangfireConfiguration.UseRedisStorage(redis, new RedisStorageOptions
            {
                Prefix = redisSetting.AppName,
                Db = redisSetting.Database,
                InvisibilityTimeout = TimeSpan.FromMinutes(5),
                ExpiryCheckInterval = TimeSpan.FromMinutes(30)
            }).WithJobExpirationTimeout(TimeSpan.FromHours(2));
            hangfireConfiguration.UseFilter(serviceProvider.GetService<GlobalHangfireJobExceptionFilterAttribute>());
            hangfireConfiguration.UseFilter(serviceProvider.GetService<UmHangfireFilterAttribute>());
        });

        services.AddHangfireServer(options => { options.SchedulePollingInterval = TimeSpan.FromSeconds(1); });
    }
}