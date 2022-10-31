using Fanex.Athena.CronJobs.App.Jobs;
using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Logs;
using Fanex.Athena.CronJobs.Core.Params;
using Fanex.Athena.CronJobs.Core.Services;
using Fanex.Athena.CronJobs.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.App.UnitTests.Jobs
{
    public class NotificateChangeSettingJobTests
    {
        private INotificationService NotificationService { get; set; }
        private IGlobalSettingService GlobalSettingService { get; set; }
        private IOptions<NotificationSetting> _options { get; set; }
        private ILogAggregate _logAggregate { get; set; }

        [SetUp]
        public void SetUp()
        {
            NotificationService = Substitute.For<INotificationService>();
            GlobalSettingService = Substitute.For<IGlobalSettingService>();
            _options = Substitute.For<IOptions<NotificationSetting>>();
            _logAggregate = Substitute.For<ILogAggregate>();
        }

        [Test]
        public async Task ExecuteAsync_WhenCall_LogAggregateReceiveCall()
        {
            //Arrange
            var setting = new List<ServiceSetting>() { new ()};
            var date = DateTime.Now;
            var listTestUserId = "1,2";
            _options.Value.ReturnsForAnyArgs(new NotificationSetting());
            GlobalSettingService.GetAllSettingAsync(default).ReturnsForAnyArgs(Task.FromResult(setting.AsEnumerable()));
            GlobalSettingService.GetTestIdsAsync(default).ReturnsForAnyArgs(Task.FromResult(listTestUserId));
            GlobalSettingService.GetLastCheckChangeSettingTime(default).ReturnsForAnyArgs(date);

            var job = new NotificateChangeSettingJob(NotificationService, GlobalSettingService, _options, _logAggregate);

            //Action
            await job.ExecuteAsync();

            //Assert
            await NotificationService.Received().NotificateChangeSettingAsync(Arg.Is<NotificationServiceParam>(_ => _.LastDate == date && _.ListTestUserIds == listTestUserId));
            _logAggregate.Received().SendLog(LogLevel.Information);
        }
    }
}
