using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Data.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Core.DTO;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository
{
    public class AccumulateRepositoryTests
    {
        private AccumulateRepository AccumulateRepository { get; set; }

        private IDynamicRepository DynamicRepository { get; set; }

        [SetUp]
        public void SetUp()
        {
            DynamicRepository = Substitute.For<IDynamicRepository>();
            AccumulateRepository = new AccumulateRepository(DynamicRepository);
        }

        [Test]
        public async Task GetUserAccumulateWinlossAsync_WhenCall_ReceiveFullParam()
        {
            var custId = "1";
            var date = DateTime.Now;

            await AccumulateRepository.GetUserAccumulateWinlossAsync(custId, date);

            await DynamicRepository.Received().FetchAsync<AccumulateWinloss>(Arg.Is<GetUserAccumulateWinlossCriteria>(_ => _.CustIds == custId && _.CurrentDate == date));
        }
    }
}
