using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Services
{
    using Core.Constants;
    using Core.DTO;
    using Core.Services;

    public class BellNotificationServiceTests
    {
        private IBellNotificationRepository _bellNotificationRepository;
        private IBellNotificationService _service;
        private IOptions<AppSetting> _options;
        private ILogAggregate _logAggregate;

        private const int OffSet = 0;

        [SetUp]
        public void SetUp()
        {
            _bellNotificationRepository = Substitute.For<IBellNotificationRepository>();
            _logAggregate = Substitute.For<ILogAggregate>();
            _options = Substitute.For<IOptions<AppSetting>>();
            _options.Value.ReturnsForAnyArgs(new AppSetting() { ServerTimeOffSet = OffSet });

            _service = new BellNotificationService(_bellNotificationRepository, _options, _logAggregate);
        }

        [TearDown]
        public void TearDown()
        {
            _bellNotificationRepository = null;
            _logAggregate = null;
            _options = null;
        }

        [Test]
        public async Task InsertBellNotificationAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var param = CreateParam();

            //Action
            await _service.InsertBellNotificationAsync(param);

            //Assert
            await _bellNotificationRepository.Received().InsertNotificationAsync(Arg.Is<InsertBellNotificationParam>(_ => IsCorrectInsertBellNotificationParam(_, param)));
        }

        [Test]
        public async Task InsertMPABellNotificationAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var param = CreateParam();

            //Action
            await _service.InsertMPABellNotificationAsync(param);

            //Assert
            await _bellNotificationRepository.Received().InsertMPANotificationAsync(Arg.Is<InsertBellNotificationParam>(_ => IsCorrectInsertBellNotificationParam(_, param)));
        }

        [TestCase(-1, 0)]
        [TestCase(1, 1)]
        public async Task GetDWHDailyJobStatusAsync_WhenCall_ReceiveCorrectData(int jobId, int expected)
        {
            //Arrange
            _bellNotificationRepository.GetDWHDailyJobStatusAsync(Arg.Any<int>()).Returns(expected);

            //Action
            var result = await _service.GetDWHDailyJobStatusAsync(jobId);

            //Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase("9999-02-01", true)]
        [TestCase("2011-02-01", false)]
        [TestCase("", false)]
        public async Task IsAbnormalBetNotificationJobDone_WhenCall_ReceiveCorrectData(string today, bool expected)
        {
            //Arrange
            _bellNotificationRepository.GetSystemSettingAsync(Arg.Any<string>())
                .Returns(new SystemSetting { Value = today });

            //Action
            var result = await _service.IsAbnormalBetNotificationJobDone();

            //Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task UpdateAbnormalBetNotificationJobDate_WhenCall_CallUpdateSystemSettingAsync()
        {
            //Arrange

            //Action
            await _service.UpdateAbnormalBetNotificationJobDate();

            //Assert
            await _bellNotificationRepository.Received().UpdateSystemSettingAsync(Arg.Is<SystemSetting>(_ => _.Name.Equals(SystemSettingName.AbnormalBetNotificationJob)));
        }

        [TestCase("Test1", "2011-01-01")]
        [TestCase("Test2", "2012-01-01")]
        [TestCase("Test3", "2013-01-01")]
        public async Task GetDailyAbnormalBetAsync_WhenCall_ReceiveCorrectData(string licenseeName, string createdDate)
        {
            //Arrange
            var expected = new AbnormalBet[] { new AbnormalBet { LicenseeName = licenseeName, CreatedDate = DateTime.Parse(createdDate) } };
            _bellNotificationRepository.GetDailyAbnormalBetAsync().Returns(expected);

            //Action
            var result = await _service.GetDailyAbnormalBetAsync();

            //Assert
            Assert.AreEqual(result, expected);
        }

        private static InsertBellNotificationParam CreateParam()
            => new()
            {
                MessageType = 2,
                LicenseeNames = "Test",
                SubjectCs = "SubjectCs",
                SubjectEn = "SubjectEn",
                ContentCs = "ContentCs",
                ContentEn = "ContentEn",
                EventDate = DateTime.Now
            };

        private static bool IsCorrectInsertBellNotificationParam(InsertBellNotificationParam actual, InsertBellNotificationParam expect)
        {
            return (actual.MessageType == expect.MessageType)
                && (actual.LicenseeNames.Equals(expect.LicenseeNames))
                && (actual.SubjectEn.Equals(expect.SubjectEn))
                && (actual.SubjectCs.Equals(expect.SubjectCs))
                && (actual.ContentCs.Equals(expect.ContentCs))
                && (actual.ContentEn.Equals(expect.ContentEn))
                && (actual.EventDate.Equals(expect.EventDate));
        }
    }
}