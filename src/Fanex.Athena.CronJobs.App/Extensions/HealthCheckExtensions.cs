using Fanex.Athena.CronJobs.App.HealthChecks;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Fanex.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddSiteHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSetting = configuration.GetSection("Redis").Get<RedisSetting>();

            return services.AddHealthChecks()
                .AddCheck<WhoAmIHealthCheck>("WhoAmI")
                .AddRedis($"{redisSetting.Server}:{redisSetting.Port},password={redisSetting.Pass}", "Redis")
                .AddMultiSqlServer(configuration);
        }

        public static void UseSiteHealthChecks(this IApplicationBuilder app, IConfiguration configuration)
        {
            Func<HttpContext, HealthReport, Task> customResponseWriter = async (context, report) =>
            {
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        status = report.Status.ToString(),
                        monitors = report.Entries.Select(e => new { name = e.Key, status = Enum.GetName(typeof(HealthStatus), e.Value.Status), values = e.Value.Data })
                    }, Formatting.Indented);

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            };

            var accessKey = configuration["HealthCheck:AccessKey"];
            app.UseWhen(context =>
                string.IsNullOrEmpty(accessKey) || (context.Request.Query.TryGetValue("AccessKey", out StringValues value) && accessKey.Equals(value)),
                     builder => builder.UseHealthChecks("/WhoAmI", options: new HealthCheckOptions
                     {
                         AllowCachingResponses = false,
                         Predicate = _ => _.Name.Equals("WhoAmI"),
                         ResponseWriter = customResponseWriter
                     }).UseHealthChecks("/healthz", options: new HealthCheckOptions
                     {
                         AllowCachingResponses = false,
                         Predicate = _ => !_.Name.Equals("WhoAmI"),
                         ResponseWriter = customResponseWriter
                     }));
        }


        private static IHealthChecksBuilder AddMultiSqlServer(this IHealthChecksBuilder services, IConfiguration configuration)
        {
            var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();
            foreach (var connectionString in connectionStrings)
            {
                var rawConnectionstring = Cryptography.DecryptConnectionString(connectionString.Value);
                services.AddSqlServer(rawConnectionstring, name: connectionString.Key);
            }
            return services;
        }
    }
}
