using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Enums;
using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private INotificationRepository NotificationRepository { get; }
        private IAccumulateRepository AccumulateRepository { get; }
        private AppSetting AppSetting { get; }
        private ILogAggregate LogAggregater { get; }


        public NotificationService(INotificationRepository notificationRepository,
            IAccumulateRepository accumulateRepository,
            IOptions<AppSetting> options,
            ILogAggregate logAggregater)
        {
            NotificationRepository = notificationRepository;
            AccumulateRepository = accumulateRepository;
            AppSetting = options?.Value;
            LogAggregater = logAggregater;
        }

        public async Task NotificateChangeSettingAsync(NotificationServiceParam param)
        {
            await LogAggregater.AppendLogAsync($"Begin Notifcate Change Setting Service. Param: {JsonConvert.SerializeObject(param)}");
            var serverDate = GetServerDate();
            var changeSettingCusts = await NotificationRepository.GetCustChangeThresholdSettingAsync(new ChangeThresholdSettingParam
            {
                IsDeposit = true,
                LastScanDate = param.LastDate,
                SportType = (int)SportType.SportBook,
                SuperList = param.ListTestUserIds,
                IsTest = param.IsTest
            });

            var changeSettingIds = GetChangeSettingCustIds(changeSettingCusts);
            await LogAggregater.AppendLogAsync($"Change setting cust from last scan date {param.LastDate}: {JsonConvert.SerializeObject(changeSettingCusts)}");

            var listReachNotifactionChangeSetting = new List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)>();

            var accumWls = await AccumulateRepository.GetUserAccumulateWinlossAsync(changeSettingIds, serverDate);
            await LogAggregater.AppendLogAsync($"Accum win loss from date {serverDate}: {JsonConvert.SerializeObject(accumWls)}");
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

            await LogAggregater.AppendLogAsync($"Insert change setting notification param: {JsonConvert.SerializeObject(insertParam)}");
            await NotificationRepository.InsertNotificationAsync(insertParam);

        }

        public async Task NotificateReachThresholdAsync(NotificationServiceParam param)
        {
            await LogAggregater.AppendLogAsync($"Begin Notifcate reach threshold. Param: {JsonConvert.SerializeObject(param)}");
            var isLastBatch = false;
            var lastDate = param.LastDate;
            while (!isLastBatch)
            {
                var accumulateWL = await AccumulateRepository.GetLastestAccumulateWinlossAsync(lastDate, param.BatchNo);
                await LogAggregater.AppendLogAsync($"Accum win loss from {lastDate}: {JsonConvert.SerializeObject(accumulateWL)}");
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
            var timezone = TimeZoneInfo.CreateCustomTimeZone("LAP DB Timezone", TimeSpan.FromHours((double)AppSetting.ServerTimeOffSet), "LAP DB Timezone", "LAP DB Timezone");
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
            var thresholdInfos = await NotificationRepository.GetCustThresholdSettingAsync(new GetSettingThresholdParam
            {
                CustList = custIds,
                IsDeposit = true,
                IsTest = param.IsTest,
                SuperList = param.ListTestUserIds,
                SportType = (int)SportType.SportBook
            });
            await LogAggregater.AppendLogAsync($"Threshold for cust in last accum winloss batch: {JsonConvert.SerializeObject(thresholdInfos)}");

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

            await LogAggregater.AppendLogAsync($"Insert reach threshold notification param: {JsonConvert.SerializeObject(insertParam)}");
            await NotificationRepository.InsertNotificationAsync(insertParam);

        }

        private static IEnumerable<CustAccumulateParam> BuildCustAccumulateParam(List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)> listReachThresholdCust)

            => listReachThresholdCust.Select(_ => new CustAccumulateParam
            {
                AccumWL = _.AccumWL.AccumWL,
                CustId = _.AccumWL.CustId,
                SportType = (int)SportType.SportBook,
                WinningThreshold = _.CustThreshold.WinningThreshold
            });

    }
}
