using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace Fanex.Athena.CronJobs.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private IDynamicRepository DynamicRepository { get; }

        public NotificationRepository(IDynamicRepository dynamicRepository)
            => DynamicRepository = dynamicRepository;

        public Task InsertNotificationAsync(InsertNotificationParam param)
        {
            var xmlReachCust = BuildReachCust(param.Customers);

            return DynamicRepository.ExecuteAsync(new NotificationCommand
            {
                CustReached = xmlReachCust,
                LastCheckTime = param.LastCheckTime,
                LastScanSettingDate = param.LastScanSettingDate,
                NotificationType = param.NotificationType,
                ServiceName = param.ServiceName
            });
        }

        private string BuildReachCust(IEnumerable<CustAccumulateParam> customers)
        {
            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("CustIds");
            xmlDoc.AppendChild(root);
            foreach (var cust in customers)
            {
                var node = xmlDoc.CreateElement("Cust");
                node.SetAttribute("CustId", cust.CustId.ToString());
                node.SetAttribute("AccumWL", cust.AccumWL.ToString());
                node.SetAttribute("SportType", cust.SportType.ToString());
                node.SetAttribute("WinningThreshold", cust.WinningThreshold.ToString());

                root.AppendChild(node);
            }

            return xmlDoc.ToString();

        }

        public Task<IEnumerable<CustThresholdSetting>> GetCustThresholdSettingAsync(GetSettingThresholdParam param)
            => DynamicRepository.FetchAsync<CustThresholdSetting>(new GetCustThresholdSettingCriteria
            {
                CustList = param.CustList,
                IsDeposit = param.IsDeposit,
                SportType = param.SportType,
                IsTest = param.IsTest,
                SuperList = param.SuperList
            });

        public Task<IEnumerable<CustThresholdSetting>> GetCustChangeThresholdSettingAsync(ChangeThresholdSettingParam param)
            => DynamicRepository.FetchAsync<CustThresholdSetting>(new GetChangeThresholdSettingCriteria
            {
                LastScanDate = param.LastScanDate,
                SuperList = param.SuperList,
                SportType = param.SportType,
                IsTest = param.IsTest,
                IsDeposit = param.IsDeposit
            });
    }
}
