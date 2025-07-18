namespace Fanex.Athena.CronJobs.Host.Startup;

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Application.Filters;
using Application.Settings;
using Configurations;
using Data.Repository;
using Data.Vault;
using Domain.Logs;
using Domain.Repository;
using Domain.Services;
using HealthChecks;
using Infrastructure.Logs;
using Infrastructure.Repository;
using Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotificationSetting>(configuration.GetSection(NotificationSetting.SettingName));
        services.Configure<BellNotificationSetting>(configuration.GetSection(BellNotificationSetting.SettingName));
        services.Configure<SentrySetting>(configuration.GetSection(SentrySetting.SettingName));
        services.Configure<RedisSetting>(configuration.GetSection(RedisSetting.SettingName));
        services.Configure<AppSetting>(configuration.GetSection(AppSetting.SettingName));
        services.Configure<LarkWebhookSetting>(configuration.GetSection(LarkWebhookSetting.SettingName));

        // DI
        services.AddRepository();
        services.AddService();
        services.AddFilter();

        services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));

        services.Configure<RouteHandlerOptions>(options => { options.ThrowOnBadRequest = true; });


        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            options.ForwardLimit = null;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });


        services.AddHttpContextAccessor();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddUnderMaintenance(configuration);
        services.AddLogAggregate();
        services.AddHealthChecks(configuration);
        services.AddHttpClient(configuration);

        services.InstallServices(configuration, typeof(IServiceInstaller).Assembly);

        return services;
    }

    private static void AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var larkWebhookSetting = configuration.GetSection(LarkWebhookSetting.SettingName).Get<LarkWebhookSetting>();
        var httpClientBuilder = services.AddHttpClient(LarkWebhookSetting.SettingName).RemoveAllLoggers();

        if (!string.IsNullOrEmpty(larkWebhookSetting.ProxyUrl))
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                Proxy = new WebProxy(new Uri(larkWebhookSetting.ProxyUrl)),
                UseProxy = true
            });
        }
    }

    private static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(RedisSetting.SettingName).Get<RedisSetting>();

        services
            .AddHealthChecks()
            .AddCheck<ServerHealthCheck>("Server")
            .AddCheck<VaultHealthCheck>("Vault")
            .AddRedis($"{redisSetting.Server}:{redisSetting.Port},password={redisSetting.Pass}", "Redis");
    }

    private static void AddRepository(this IServiceCollection services)
    {
        services.AddSingleton<IDynamicRepository, DynamicRepository>();
        services.AddScoped<IAccumulateRepository, AccumulateRepository>();
        services.AddScoped<IGlobalSettingRepository, GlobalSettingRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IBellNotificationRepository, BellNotificationRepository>();
        services.AddScoped<IMatchMonitoringCenterRepository, MatchMonitoringCenterRepository>();
    }

    private static void AddService(this IServiceCollection services)
    {
        services.AddSingleton<VaultDbAliasService, VaultDbAliasServiceImp>();
        services.AddScoped<IGlobalSettingService, GlobalSettingService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IBellNotificationService, BellNotificationService>();
        services.AddScoped<IMatchMonitoringCenterService, MatchMonitoringCenterService>();
    }

    private static void AddFilter(this IServiceCollection services)
    {
        services.AddSingleton<GlobalHangfireJobExceptionFilterAttribute>();
        services.AddSingleton<UmHangfireFilterAttribute>();
    }

    private static void AddUnderMaintenance(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();
        services.AddSingleton<IHttpContextHelper, ContextInMemoryHelper>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
    }

    private static void AddLogAggregate(this IServiceCollection services)
    {
        services.AddScoped<ILogAggregate, LogAggregate>();
    }

    private static void InstallServices(this IServiceCollection services, IConfiguration configuration, params Assembly[] assemblies)
    {
        var serviceInstallers = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && t.IsClass)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var serviceInstaller in serviceInstallers)
        {
            serviceInstaller.Install(services, configuration);
        }
    }

    internal class ContextInMemoryHelper(IHttpContextAccessor httpContextAccessor, IMemoryCache cache) : IHttpContextHelper
    {
        private readonly TimeSpan byPassUmCacheTime = TimeSpan.FromMinutes(30);

        public string GetSession(string key)
            => cache.TryGetValue(BuildKey(key), out string value) ? value : string.Empty;

        public string GetValueFromQueryString(string queryKey)
            => httpContextAccessor.HttpContext?.Request.Query[queryKey].ToString();

        public void SetSession(string key, string value)
            => cache.Set(BuildKey(key), value, byPassUmCacheTime);

        private string BuildKey(string key)
            => $"{key}:{httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString()}";
    }
}