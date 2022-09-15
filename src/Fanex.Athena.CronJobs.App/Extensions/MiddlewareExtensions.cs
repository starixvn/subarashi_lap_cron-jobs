using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using System;

namespace Fanex.Athena.CronJobs.App.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void AddUnderMaintenance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextHelper, ContextInMemoryHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
        }

        public static void UsePing(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/ping"))
                {
                    return;
                }
                await next();
            });
        }

        public static void UseErrorHandler(this IApplicationBuilder app)
        {
            //mvc
            app.UseWhen(context => !context.Request.IsAjaxRequest() && !context.Request.IsStaticRequest(),
                builder => builder.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}"));

            //api
            app.UseWhen(context => context.Request.IsAjaxRequest(),
                builder => builder.UseStatusCodePages(async context =>
                {
                    var response = context.HttpContext.Response;
                    response.ContentType = "application/json";
                    var errorObject = new
                    {
                        statusCode = response.StatusCode,
                        msg = "An error occurred while processing your request",
                        isSuccess = false
                    };
                    await response.WriteAsync(JsonConvert.SerializeObject(errorObject));
                }));

            //logging
            app.UseMiddleware<ErrorHandler>();
        }
    }

    internal class ContextInMemoryHelper : IHttpContextHelper
    {
        private readonly TimeSpan ByPassUMCacheTime = TimeSpan.FromMinutes(30);
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMemoryCache cache;

        public ContextInMemoryHelper(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.cache = cache;
        }

        public string GetSession(string key)
            => cache.TryGetValue(BuildKey(key), out string value) ? value : string.Empty;

        public string GetValueFromQueryString(string queryKey)
            => httpContextAccessor.HttpContext?.Request.Query[queryKey].ToString();

        public void SetSession(string key, string value)
            => cache.Set(BuildKey(key), value, ByPassUMCacheTime);

        private string BuildKey(string key)
            => $"{key}:{httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString()}";
    }
}
