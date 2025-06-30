namespace Fanex.Athena.CronJobs.Host.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration);
}