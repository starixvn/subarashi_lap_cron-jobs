using Fanex.Athena.CronJobs.Core.DTO;
using Fanex.Athena.CronJobs.Core.Repository;
using Fanex.Athena.CronJobs.Infrastructure.Criteria.Query;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Infrastructure.Repository
{
    public class AccumulateRepository : IAccumulateRepository
    {
        private IDynamicRepository DynamicRepository { get; }

        public AccumulateRepository(IDynamicRepository dynamicRepository)
            => DynamicRepository = dynamicRepository;

        public Task<IEnumerable<AccumulateWinloss>> GetUserAccumulateWinlossAsync(string custId, DateTime date)
            => DynamicRepository.FetchAsync<AccumulateWinloss>(new GetUserAccumulateWinlossCriteria { CurrentDate = date, CustIds = custId });


        public Task<IEnumerable<AccumulateWinloss>> GetLastestAccumulateWinlossAsync(DateTime lastCheckTime, int batchCount)
            => DynamicRepository.FetchAsync<AccumulateWinloss>(new GetLastestAccumulateCriteria
            {
                BatchNo = batchCount,
                LastCheckTIme = lastCheckTime
            });
    }
}
