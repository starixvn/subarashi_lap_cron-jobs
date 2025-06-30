namespace Fanex.Athena.CronJobs.Infrastructure.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Settings;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Repository;
using Microsoft.Extensions.Options;

public class GlobalSettingRepository(IDynamicRepository dynamicRepository, IOptions<AppSetting> appOptions) : IGlobalSettingRepository
{
    private AppSetting AppSetting { get; } = appOptions?.Value;

    public Task<IEnumerable<ServiceSetting>> GetSetting(string serviceName)
        => dynamicRepository.FetchAsync<ServiceSetting>(new GlobalSettingCriteria(!AppSetting.IsMainService) { ServiceName = serviceName });
}