using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Data.Repository;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository
{
    public class NotificationRepositoryTests
    {
        private IDynamicRepository _dynamicRepository;

        [SetUp]
        public void SetUp()
        {
            _dynamicRepository = Substitute.For<IDynamicRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _dynamicRepository = null;
        }

        [Test]
        public async Task InsertNotificationAsync_WhenCallWithNullCustomers_ReceiveXmlWithNoChild()
        {
            //Arrange
            var repo = new NotificationRepository(_dynamicRepository);
            var param = BuildInsertParam(null);
            AssignCommonProps(param);

            //Action
            await repo.InsertNotificationAsync(param);

            //Assert
            await _dynamicRepository.Received().ExecuteAsync(Arg.Is<NotificationCommand>(_ => CorrectRootName(_.CustReached) && NoChildXml(_.CustReached)));
        }

        [Test]
        public async Task InsertNotificationAsync_WhenCallWithEmptyCustomers_ReceiveXmlWithNoChild()
        {
            //Arrange
            var repo = new NotificationRepository(_dynamicRepository);
            var param = BuildInsertParam(new List<CustAccumulateParam>());
            AssignCommonProps(param);

            //Action
            await repo.InsertNotificationAsync(param);

            //Assert
            await _dynamicRepository.Received().ExecuteAsync(Arg.Is<NotificationCommand>(_ => CorrectRootName(_.CustReached) && NoChildXml(_.CustReached)));
        }

        [Test]
        public async Task InsertNotificationAsync_WhenCallWithNormalCustomers_ReceiveXmlHaveChild()
        {
            //Arrange
            var repo = new NotificationRepository(_dynamicRepository);
            var reachCustomers = new List<CustAccumulateParam>
            {
               new CustAccumulateParam { AccumWL = 1000, CustId = 1, SportType = 0, WinningThreshold = 1000 }
            };
            var param = BuildInsertParam(reachCustomers);
            AssignCommonProps(param);

            //Action
            await repo.InsertNotificationAsync(param);

            //Assert
            await _dynamicRepository.Received().ExecuteAsync(Arg.Is<NotificationCommand>(_ => CorrectRootName(_.CustReached)
                && !NoChildXml(_.CustReached)
                && XmlHaveChild(_.CustReached, reachCustomers.First())
                ));
        }

        [Test]
        public async Task GetCustThresholdSettingAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var repo = new NotificationRepository(_dynamicRepository);
            var param = new GetSettingThresholdParam
            {
                CustList = "list",
                IsDeposit = true,
                IsTest = true,
                SportType = 0,
                SuperList = "test"
            };

            //Action
            await repo.GetCustThresholdSettingAsync(param);

            //Assert
            await _dynamicRepository.Received().FetchAsync<CustThresholdSetting>(Arg.Is<GetCustThresholdSettingCriteria>(_ =>
                        _.CustList == param.CustList
                       && _.IsTest == param.IsTest
                       && _.SportType == param.SportType
                       && _.SuperList == param.SuperList
                       && _.IsDeposit == param.IsDeposit
            ));
        }

        [Test]
        public async Task GetCustChangeThresholdSettingAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var repo = new NotificationRepository(_dynamicRepository);
            var param = new ChangeThresholdSettingParam
            {
                IsDeposit = true,
                IsTest = true,
                SportType = 0,
                SuperList = "test",
                LastScanDate = DateTime.Now,
            };

            //Action
            await repo.GetCustChangeThresholdSettingAsync(param);

            //Assert
            await _dynamicRepository.Received().FetchAsync<CustThresholdSetting>(Arg.Is<GetChangeThresholdSettingCriteria>(_ =>
                         _.IsTest == param.IsTest
                       && _.SportType == param.SportType
                       && _.SuperList == param.SuperList
                       && _.IsDeposit == param.IsDeposit
                       && _.LastScanDate == param.LastScanDate
            ));
        }

        private static void AssignCommonProps(InsertNotificationParam param)
        {
            param.ServiceName = "Test";
            param.NotificationType = 1;
            param.LastCheckTime = new DateTime(2022, 1, 1);
            param.LastScanSettingDate = new DateTime(2022, 1, 1);
        }

        private static InsertNotificationParam BuildInsertParam(IEnumerable<CustAccumulateParam> customers)
            => new()
            {
                Customers = customers
            };

        private static bool NoChildXml(string xml)
        {
            var xmlDoc = XDocument.Parse(xml);
            return !xmlDoc.Root.HasElements;
        }

        private static bool CorrectRootName(string xml)
        {
            var xmlDoc = XDocument.Parse(xml);
            return string.Equals(xmlDoc.Root.Name.LocalName, "CustIds", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool XmlHaveChild(string xml, CustAccumulateParam accum)
        {
            var xmlDoc = XDocument.Parse(xml);
            var root = xmlDoc.Root;
            if (!root.HasElements)
            {
                return false;
            }

            var elems = root.Elements("Cust");
            return elems.Any(_ => _.Attribute("CustId").Value == accum.CustId.ToString()
                                && _.Attribute("AccumWL").Value == accum.AccumWL.ToString()
                                && _.Attribute("SportType").Value == accum.SportType.ToString()
                                && _.Attribute("WinningThreshold").Value == accum.WinningThreshold.ToString()
            );
        }
    }
}
