using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Data.Repository;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.UnitTest.Repository
{
    public class GlobalSettingRepositoryTests
    {
        private IDynamicRepository _dynamicRepository;

        [SetUp]
        public void Setup()
        {
            _dynamicRepository = Substitute.For<IDynamicRepository>();
        }

        [TearDown]
        public void TearDown()
        {
            _dynamicRepository = null;
        }

        [Test]
        public async Task GetSetting_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var repo = new GlobalSettingRepository(_dynamicRepository);
            var serviceName = "Test";

            //Action
            await repo.GetSetting(serviceName);

            //Assert
            await _dynamicRepository.Received().FetchAsync<ServiceSetting>(Arg.Is<GlobalSettingCriteria>(_ => string.Equals(_.ServiceName, serviceName, System.StringComparison.InvariantCulture)));
        }
    }
}
