namespace Fanex.Athena.CronJobs.Infrastructure.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Settings;
using Criteria.Command;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Repository;
using Microsoft.Extensions.Options;

public class MatchMonitoringCenterRepository(IDynamicRepository dynamicRepository, IOptions<AppSetting> options) : IMatchMonitoringCenterRepository
{
    private AppSetting AppSetting { get; } = options?.Value;

    public Task<string> DetectAlertRulesAsync()
    {
        return dynamicRepository.GetAsync<string>(new DetectAlertRulesCriteria());
    }

    public Task UpdateMatchAlertAsync(string matchMonitoringData)
    {
        var command = new UpdateMatchAlertCommand
        {
            MatchMonitoringData = matchMonitoringData
        };

        return dynamicRepository.ExecuteAsync(command);
    }

    public async Task UpdateAlertHistoryAsync(string matchMonitoringData)
    {
        var command = new UpdateAlertHistoryCommand
        {
            MatchMonitoringData = matchMonitoringData
        };

        await dynamicRepository.ExecuteAsync(command);

        if (IsProductionOrUat())
        {
            var commandStandby = new UpdateAlertHistoryCommand(isStandby: true)
            {
                MatchMonitoringData = matchMonitoringData
            };

            await dynamicRepository.ExecuteAsync(commandStandby);
        }
    }

    public Task<IEnumerable<TriggeredDataSendWebHook>> GetTriggeredDataSendWebHookAsync(string licenseeName, string matchMonitoringData)
    {
        var criteria = new GetTriggeredDataSendWebHookCriteria
        {
            LicenseeName = licenseeName,
            MatchMonitoringData = matchMonitoringData
        };

        return dynamicRepository.FetchAsync<TriggeredDataSendWebHook>(criteria);
    }

    private bool IsProductionOrUat()
    {
        var env = AppSetting.Environment.ToUpperInvariant();
        return env.StartsWith("UAT") || env.StartsWith("PRO");
    }
}