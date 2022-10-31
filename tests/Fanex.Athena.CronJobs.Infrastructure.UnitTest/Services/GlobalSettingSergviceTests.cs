using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.UnitTest.Services
{
    public class GlobalSettingSergviceTests
    {
        private const string LAST_CHECK_TIME_SETTING_NAME = "LastCheckTime";
        private const string LAST_SCAN_SETTING_DATE = "LastScanSettingDate";
        private const string FULL_DATETIME_STRING = "yyyy-MM-dd HH:mm:ss.fffffff";

        private IGlobalSettingRepository SettingRepository { get; set; }

        [SetUp]
        public void SetUp()
        {
            SettingRepository = Substitute.For<IGlobalSettingRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            SettingRepository = null;
        }

        [Test]
        public async Task GetAllSettingAsync_WhenCall_RepoReveiceCorrectParam()
        {
            //Arrange
            var service = CreateService();
            var serviceName = "test";


            //Action
            await service.GetAllSettingAsync(serviceName);

            //Assert
            await SettingRepository.Received().GetSetting(Arg.Is<string>(serviceName));
        }

        [Test]
        public void GetLastCheckReachThresholdTime_WhenCallWithNullParam_ThrowException()
        {
            //Arrange
            var service = CreateService();

            //Assert
             Assert.Throws<ArgumentNullException>(() => service.GetLastCheckReachThresholdTime(null));
        }

        [Test]
        public void GetLastCheckReachThresholdTime_WhenCallAndFindNoLastCheckTimeSettingValue_ThrowInvalidCastException()
        {
            //Arrange
            var service = CreateService();
            var settings = new List<ServiceSetting>();

            //Assert
            Assert.Throws<InvalidCastException>(() => service.GetLastCheckReachThresholdTime(settings));
        }

        [Test]
        public void GetLastCheckReachThresholdTime_WhenCallWithNormalSetting_ReturnCorrectValue()
        {
            //Arrange
            var service = CreateService();
            var date = DateTime.Now;
            var settings = new List<ServiceSetting>
            {
                new ServiceSetting{ SettingName = LAST_CHECK_TIME_SETTING_NAME, SettingValue = date.ToString(FULL_DATETIME_STRING)}
            };

            //Action
            var result = service.GetLastCheckReachThresholdTime(settings);

            //Assert
            Assert.That(result, Is.EqualTo(date));
        }

        [Test]
        public void GetLastCheckChangeSettingTime_WhenCallWithNullParam_ThrowException()
        {
            //Arrange
            var service = CreateService();

            //Assert
            Assert.Throws<ArgumentNullException>(() => service.GetLastCheckChangeSettingTime(null));
        }

        [Test]
        public void GetLastCheckChangeSettingTime_WhenCallAndFindNoLastCheckTimeSettingValue_ThrowInvalidCastException()
        {
            //Arrange
            var service = CreateService();
            var settings = new List<ServiceSetting>();

            //Assert
            Assert.Throws<InvalidCastException>(() => service.GetLastCheckChangeSettingTime(settings));
        }

        [Test]
        public void GetLastCheckChangeSettingTime_WhenCallWithNormalSetting_ReturnCorrectValue()
        {
            //Arrange
            var service = CreateService();
            var date = DateTime.Now;
            var settings = new List<ServiceSetting>
            {
                new ServiceSetting{ SettingName = LAST_SCAN_SETTING_DATE, SettingValue = date.ToString(FULL_DATETIME_STRING)}
            };

            //Action
            var result = service.GetLastCheckChangeSettingTime(settings);

            //Assert
            Assert.That(result, Is.EqualTo(date));
        }

        [Test]
        public async Task GetLastCheckChangeSettingTimeAsync_WhenCall_RepoReceiveCorrectParam()
        {
            // Arrange
            var service = CreateService();
            var serviceName = "test";
            var param = GetServiceSettings();
            SettingRepository.GetSetting(serviceName).Returns(param);

            //Action
            await service.GetLastCheckChangeSettingTimeAsync(serviceName);

            //Assert
            await SettingRepository.Received().GetSetting(Arg.Is<string>(serviceName));
        }

        [Test]
        public async Task GetLastCheckReachThresholdTimeAsync_WhenCall_RepoReceiveCorrectParam()
        {
            // Arrange
            var service = CreateService();
            var serviceName = "test";
            var param = GetServiceSettings();
            SettingRepository.GetSetting(serviceName).Returns(param);

            //Action
            await service.GetLastCheckReachThresholdTimeAsync(serviceName);

            //Assert
            await SettingRepository.Received().GetSetting(Arg.Is<string>(serviceName));
        }

        [Test]
        public async Task GetTestIdsAsync_WhenCall_RepoReceiveCorrectParam()
        {
            // Arrange
            var service = CreateService();
            var serviceName = "test";

            //Action
            await service.GetTestIdsAsync(serviceName);

            //Assert
            await SettingRepository.Received().GetSetting(Arg.Is<string>(serviceName));
        }

        [Test]
        public void GetTestIds_WhenCallWithNullParam_ThrowException()
        {
            //Arrange
            var service = CreateService();

            //Assert
            Assert.Throws<ArgumentNullException>(() => service.GetTestIds(null));
        }

        [Test]
        public void GetTestIds_WhenCallWithNoSetting_ReturnNullOrEmptyValue()
        {
            // Arrange
            var service = CreateService();
            var settings = new List<ServiceSetting>();

            //Action
            var result = service.GetTestIds(settings);

            //Assert
            Assert.That(string.IsNullOrEmpty(result), Is.True);
        }

        private GlobalSettingService CreateService()
            => new(SettingRepository);

        private static IEnumerable<ServiceSetting> GetServiceSettings()
            => new List<ServiceSetting>
            {
                new(){ SettingName = LAST_CHECK_TIME_SETTING_NAME, SettingValue = "2022-01-01"},
                new(){ SettingName = LAST_SCAN_SETTING_DATE, SettingValue = "2022-01-01" }
            };
    }
}
