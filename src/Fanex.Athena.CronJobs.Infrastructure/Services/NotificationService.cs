namespace Fanex.Athena.CronJobs.Infrastructure.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Settings;
using Domain.DTO;
using Domain.Enums;
using Domain.Logs;
using Domain.Params;
using Domain.Repository;
using Domain.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class NotificationService(
    INotificationRepository notificationRepository,
    IAccumulateRepository accumulateRepository,
    IOptions<AppSetting> options,
    ILogAggregate logAggregater)
    : INotificationService
{
    private AppSetting AppSetting { get; } = options?.Value;

    public async Task NotificateChangeSettingAsync(NotificationServiceParam param)
    {
        logAggregater.AppendHeader("Begin Notifcate Change Setting Service");
        logAggregater.AppendLog($"Param: {JsonConvert.SerializeObject(param)}");
        var serverDate = GetServerDate();
        var changeSettingCusts = await notificationRepository.GetCustChangeThresholdSettingAsync(new ChangeThresholdSettingParam
        {
            IsDeposit = true,
            LastScanDate = param.LastDate,
            SportType = (int)SportType.SportBook,
            SuperList = param.ListTestUserIds,
            IsTest = param.IsTest
        });

        var changeSettingIds = GetChangeSettingCustIds(changeSettingCusts);
        logAggregater.AppendLog($"Change setting cust from last scan date {param.LastDate}: {JsonConvert.SerializeObject(changeSettingCusts)}");

        var listReachNotifactionChangeSetting = new List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)>();

        var accumWls = await accumulateRepository.GetUserAccumulateWinlossAsync(changeSettingIds, serverDate);
        logAggregater.AppendLog($"Accum win loss from date {serverDate}: {JsonConvert.SerializeObject(accumWls)}");
        foreach (var accumWl in accumWls)
        {
            var cust = changeSettingCusts.FirstOrDefault(_ => _.CustId == accumWl.CustId);
            if (accumWl.AccumWL >= cust?.WinningThreshold)
            {
                listReachNotifactionChangeSetting.Add((accumWl, cust));
            }
        }

        var custAccumulateParam = BuildCustAccumulateParam(listReachNotifactionChangeSetting);
        var insertParam = new InsertNotificationParam
        {
            Customers = custAccumulateParam,
            LastScanSettingDate = serverDate,
            ServiceName = param.ServiceName,
            NotificationType = (int)NotificationType.ChangeThresholdSetting
        };

        logAggregater.AppendLog($"Insert change setting notification param: {JsonConvert.SerializeObject(insertParam)}");
        await notificationRepository.InsertNotificationAsync(insertParam);
    }

    public async Task NotificateReachThresholdAsync(NotificationServiceParam param)
    {
        logAggregater.AppendHeader("Begin Notifcate Reach Threshold Service");
        logAggregater.AppendLog($"Param: {JsonConvert.SerializeObject(param)}");
        var isLastBatch = false;
        var lastDate = param.LastDate;
        while (!isLastBatch)
        {
            var accumulateWL = await accumulateRepository.GetLastestAccumulateWinlossAsync(lastDate, param.BatchNo);
            logAggregater.AppendLog($"Accum win loss from {lastDate}: {JsonConvert.SerializeObject(accumulateWL)}");
            if (!accumulateWL.Any())
            {
                return;
            }

            await NotificateReachThresholdCustAsync(accumulateWL, param);
            isLastBatch = param.IsRunOnlyFirstBatch || accumulateWL.Count() < param.BatchNo;
            lastDate = accumulateWL.Max(_ => _.UpdatedTime);
        }
    }

    private static string GetChangeSettingCustIds(IEnumerable<CustThresholdSetting> changeSettingCusts)
    {
        if (changeSettingCusts == null || !changeSettingCusts.Any())
        {
            return string.Empty;
        }

        return string.Join(",", changeSettingCusts.Select(_ => _.CustId));
    }

    private DateTime GetServerDate()
    {
        var timezone = TimeZoneInfo.CreateCustomTimeZone("LAP DB Timezone", TimeSpan.FromHours(AppSetting.ServerTimeOffSet), "LAP DB Timezone",
            "LAP DB Timezone");
        var date = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
        return date;
    }

    private async Task NotificateReachThresholdCustAsync(IEnumerable<AccumulateWinloss> accumulateWLs, NotificationServiceParam param)
    {
        if (!accumulateWLs.Any())
        {
            return;
        }

        var listReachThresholdCust = new List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)>();

        var custIds = string.Join(",", accumulateWLs.Select(_ => _.CustId));
        var thresholdInfos = await notificationRepository.GetCustThresholdSettingAsync(new GetSettingThresholdParam
        {
            CustList = custIds,
            IsDeposit = true,
            IsTest = param.IsTest,
            SuperList = param.ListTestUserIds,
            SportType = (int)SportType.SportBook
        });
        logAggregater.AppendLog($"Threshold for cust in last accum winloss batch: {JsonConvert.SerializeObject(thresholdInfos)}");

        foreach (var info in thresholdInfos)
        {
            var accumWL = accumulateWLs.FirstOrDefault(_ => _.CustId == info.CustId);
            if (accumWL?.AccumWL >= info.WinningThreshold)
            {
                listReachThresholdCust.Add((accumWL, info));
            }
        }

        var custAccumulateParams = BuildCustAccumulateParam(listReachThresholdCust);
        var lastCheckTime = accumulateWLs.Max(_ => _.UpdatedTime);

        var insertParam = new InsertNotificationParam
        {
            Customers = custAccumulateParams,
            LastCheckTime = lastCheckTime,
            ServiceName = param.ServiceName,
            NotificationType = (int)NotificationType.ReachThreshold
        };

        logAggregater.AppendLog($"Insert reach threshold notification param: {JsonConvert.SerializeObject(insertParam)}");
        await notificationRepository.InsertNotificationAsync(insertParam);
    }

    private static IEnumerable<CustAccumulateParam> BuildCustAccumulateParam(
        List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)> listReachThresholdCust)
        => listReachThresholdCust.Select(_ => new CustAccumulateParam
        {
            AccumWL = _.AccumWL.AccumWL,
            CustId = _.AccumWL.CustId,
            SportType = (int)SportType.SportBook,
            WinningThreshold = _.CustThreshold.WinningThreshold
        });
}