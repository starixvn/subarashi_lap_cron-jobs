namespace Fanex.Athena.CronJobs.Domain.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTO;

public interface IAccumulateRepository
{
    Task<IEnumerable<AccumulateWinloss>> GetLastestAccumulateWinlossAsync(DateTime lastCheckTime, int batchCount);

    Task<IEnumerable<AccumulateWinloss>> GetUserAccumulateWinlossAsync(string custId, DateTime date);
}