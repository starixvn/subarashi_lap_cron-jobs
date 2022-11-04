using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Athena.CronJobs.Infrastructure.Repository;
using Fanex.Data.Repository;
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
        public async Task GetUserAccumulateWinlossAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var custId = "1";
            var date = DateTime.Now;

            //Action
            await AccumulateRepository.GetUserAccumulateWinlossAsync(custId, date);

            //Assert
            await DynamicRepository.Received().FetchAsync<AccumulateWinloss>(Arg.Is<GetUserAccumulateWinlossCriteria>(_ => _.CustIds == custId && _.CurrentDate == date));
        }

        [Test]
        public async Task GetLastestAccumulateWinlossAsync_WhenCall_ReceiveCorrectParam()
        {
            //Arrange
            var date = DateTime.Now;
            var batchCount = 2000;

            //Action
            await AccumulateRepository.GetLastestAccumulateWinlossAsync(date, batchCount);

            //Assert
            await DynamicRepository.Received().FetchAsync<AccumulateWinloss>(Arg.Is<GetLastestAccumulateCriteria>(_ => _.LastCheckTIme == date && _.BatchNo == batchCount));
        }
    }
}
