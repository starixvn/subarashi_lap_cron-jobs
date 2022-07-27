using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Enums;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Core.Services;
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


        public NotificationService(INotificationRepository notificationRepository, 
            IAccumulateRepository accumulateRepository)
        {
            NotificationRepository = notificationRepository;
            AccumulateRepository = accumulateRepository;
        }

        public async Task NotificateChangeSettingAsync(NotificationServiceParam param)
        {
            var changeSettingCusts = await NotificationRepository.GetCustChangeThresholdSettingAsync(new ChangeThresholdSettingParam
            {
                IsDeposit = true,
                LastScanDate = param.LastDate,
                SportType = (int)SportType.SportBook,
                SuperList = param.ListTestUserIds,
                IsTest = param.IsTest
            });

            var custIds = string.Join(",", changeSettingCusts.Select(_ => _.CustId));

            var listReachNotifactionChangeSetting = new List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)>();

            var accumWls = await AccumulateRepository.GetUserAccumulateWinlossAsync(custIds, DateTime.Now);
            foreach (var cust in changeSettingCusts)
            {
                var accumWl = accumWls.FirstOrDefault(_ => _.CustId == cust.CustId);
                if(accumWl?.AccumWL > cust.WinningThreshold)
                {
                    listReachNotifactionChangeSetting.Add((accumWl, cust));
                }
            }

            var custAccumulateParam = BuildCustAccumulateParam(listReachNotifactionChangeSetting);
            var maxDateTime = accumWls.Max(_ => _.UpdatedTime);

            await NotificationRepository.InsertNotificationAsync(new InsertNotificationParam
            {
                Customers = custAccumulateParam,
                LastScanSettingDate = maxDateTime,
                NotificationType = (int)NotificationType.ChangeThresholdSetting
            });
        }

        public async Task NotificateReachThresholdAsync(NotificationServiceParam param)
        {
            var isLastBatch = false;
            var lastDate = param.LastDate;
            while(!isLastBatch)
            {
                var accumulateWL = await AccumulateRepository.GetLastestAccumulateWinlossAsync(lastDate, param.BatchNo);
                await NotificateReachThresholdCustAsync(accumulateWL, param);
                isLastBatch = param.IsRunOnlyFirstBatch || accumulateWL.Count() < param.BatchNo;
                lastDate = accumulateWL.Max(_ => _.UpdatedTime);
            }

        }

        private async Task NotificateReachThresholdCustAsync(IEnumerable<AccumulateWinloss> accumulateWLs, NotificationServiceParam param)
        {
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

            foreach (var info in thresholdInfos)
            {
                var accumWL = accumulateWLs.FirstOrDefault(_ => _.CustId == info.CustId);
                if ( accumWL?.AccumWL > info.WinningThreshold)
                {
                    listReachThresholdCust.Add((accumWL, info));
                }
            }

            var custAccumulateParams = BuildCustAccumulateParam(listReachThresholdCust);
            var lastCheckTime = accumulateWLs.Max(_ => _.UpdatedTime);
            await NotificationRepository.InsertNotificationAsync(new InsertNotificationParam 
            {
                Customers = custAccumulateParams,
                LastCheckTime = lastCheckTime,
                NotificationType = (int)NotificationType.ReachThreshold
            });

        }

        private IEnumerable<CustAccumulateParam> BuildCustAccumulateParam(List<(AccumulateWinloss AccumWL, CustThresholdSetting CustThreshold)> listReachThresholdCust)
        
            => listReachThresholdCust.Select(_ => new CustAccumulateParam
            {
                AccumWL = _.AccumWL.AccumWL,
                CustId = _.AccumWL.CustId,
                SportType = (int)SportType.SportBook,
                WinningThreshold = _.CustThreshold.WinningThreshold
            });
        
    }
}
