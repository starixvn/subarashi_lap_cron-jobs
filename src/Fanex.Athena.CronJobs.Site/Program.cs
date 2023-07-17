using Fanex.Athena.CronJobs.Infrastructure.Logs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sentry;
using System;

namespace Fanex.Athena.CronJobs.Site
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Cron Jobs";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry(option =>
                    {
                        option.BeforeSend = @event =>
                        {
                            if (@event.Level == Sentry.Protocol.SentryLevel.Info && @event.Logger.Contains(nameof(LogAggregate)) && @event.LogEntry.Message is not null)
                            {
                                var header = @event.LogEntry.Message.Split(Environment.NewLine)[0];
                                @event.SetFingerprint(new[] { @event.Environment, header, @event.Level.ToString() });
                            }

                            return @event;
                        };
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
