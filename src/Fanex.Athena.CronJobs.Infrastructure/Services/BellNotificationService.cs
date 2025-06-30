namespace Fanex.Athena.CronJobs.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Settings;
using Domain.Constants;
using Domain.DTO;
using Domain.Params;
using Domain.Repository;
using Domain.Services;
using Microsoft.Extensions.Options;

public class BellNotificationService(
    IBellNotificationRepository bellNotificationRepository,
    IOptions<AppSetting> options)
    : IBellNotificationService
{
    private AppSetting AppSetting { get; } = options?.Value;

    public async Task InsertBellNotificationAsync(InsertBellNotificationParam param)
    {
        await bellNotificationRepository.InsertNotificationAsync(param);
    }

    public async Task InsertMPABellNotificationAsync(InsertBellNotificationParam param)
    {
        await bellNotificationRepository.InsertMPANotificationAsync(param);
    }

    public async Task<int> GetDWHDailyJobStatusAsync(int jobId)
    {
        return await bellNotificationRepository.GetDWHDailyJobStatusAsync(jobId);
    }

    public async Task<bool> IsAbnormalBetNotificationJobDone()
    {
        var today = GetServerDate().ToString("yyyy-MM-dd");
        var jobDoneDate = await bellNotificationRepository.GetSystemSettingAsync(SystemSettingName.AbnormalBetNotificationJob);

        if (string.IsNullOrEmpty(jobDoneDate.Value) || DateTime.Parse(today) > DateTime.Parse(jobDoneDate.Value))
        {
            return false;
        }

        return true;
    }

    public async Task UpdateAbnormalBetNotificationJobDate()
    {
        var today = GetServerDate().ToString("yyyy-MM-dd");
        await bellNotificationRepository.UpdateSystemSettingAsync(new SystemSetting
        {
            Name = SystemSettingName.AbnormalBetNotificationJob,
            Value = today
        });
    }

    public async Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync()
    {
        return await bellNotificationRepository.GetDailyAbnormalBetAsync();
    }

    private DateTime GetServerDate()
    {
        var timezone = TimeZoneInfo.CreateCustomTimeZone("LAP DB Timezone", TimeSpan.FromHours(AppSetting.ServerTimeOffSet), "LAP DB Timezone",
            "LAP DB Timezone");
        var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
        return date;
    }
}