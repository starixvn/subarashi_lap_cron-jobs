using System;
using Fanex.Athena.CronJobs.Application.Helpers;
using Fanex.Athena.CronJobs.Host.Startup;
using Fanex.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration;
    var environment = builder.Environment;

    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddDependencyInjection(configuration);

    var app = builder.Build();

    LogService.Info("Startup application");

    if (environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }

    app.MapGet("/ping", () => "pong");

    app.UseForwardedHeaders();
    app.UseHttpsRedirection();

    app.UseHealthChecks(configuration);
    app.UseStaticFiles();
    app.UseHangFire("/jobs", configuration);

    app.UseRouting();
    app.UseCors();

    app.MapRazorPages();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    LogService.Critical("Unhandled exception during startup", ErrorMessageHelper.GenerateMessage(ex));
}