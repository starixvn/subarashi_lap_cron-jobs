namespace Fanex.Athena.CronJobs.Host.Configurations;

using System;
using System.Collections.Generic;
using Application.Settings;
using Infrastructure.Logs;
using Logging;
using Logging.Sentry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sentry;
using Sentry.Protocol;
using Serilog;
using ILogger = Logging.ILogger;

public class LoggingServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        var sentrySetting = configuration.GetSection(SentrySetting.SettingName).Get<SentrySetting>();
        var appSetting = configuration.GetSection(AppSetting.SettingName).Get<AppSetting>();

        // Sentry
        LogManager
            .SetDefaultLogCategory(sentrySetting.Environment)
            .Use(new List<ILogEngine>
            {
                new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(sentrySetting.Dsn),
                    Environment = sentrySetting.Environment,
                    MaxBreadcrumbs = sentrySetting.MaxBreadcrumbs,
                    SendDefaultPii = sentrySetting.SendDefaultPii,
                    AttachStacktrace = sentrySetting.AttachStackTrace,
                    Debug = sentrySetting.Debug,
                    BeforeSend = @event =>
                    {
                        if (@event.Level == SentryLevel.Info
                            && @event.Logger != null
                            && @event.Logger.Contains(nameof(LogAggregate))
                            && @event.LogEntry.Message is not null)
                        {
                            var header = @event.LogEntry.Message.Split(Environment.NewLine)[0];
                            @event.SetFingerprint([@event.Environment, header, @event.Level.ToString()]);
                        }

                        return @event;
                    }
                })
            });


        // Serilog
        services.AddSerilog((s, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(s)
            .Enrich.FromLogContext()
            .Filter.ByExcluding("RequestPath like '/health%'")
            .WriteTo.Console()
            .Enrich.WithProperty("Team", "Subarashi")
            .Enrich.WithProperty("Application", "LAP Cron Jobs")
            .Enrich.WithProperty("Environment", appSetting.Environment));

        services.TryAddSingleton<ILogger, Logger>();
    }
}