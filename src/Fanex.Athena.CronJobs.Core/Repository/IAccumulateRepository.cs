using Fanex.Athena.CronJobs.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fanex.Athena.CronJobs.Core.Repository
{
    public interface IAccumulateRepository
    {
        Task<IEnumerable<AccumulateWinloss>> GetLastestAccumulateWinlossAsync(DateTime lastCheckTime, int batchCount);

        Task<IEnumerable<AccumulateWinloss>> GetUserAccumulateWinlossAsync(string custId, DateTime date);
    }
}
