namespace Fanex.Athena.CronJobs.Infrastructure.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Settings;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Repository;
using Microsoft.Extensions.Options;

public class AccumulateRepository(IDynamicRepository dynamicRepository, IOptions<AppSetting> appOptions) : IAccumulateRepository
{
    private AppSetting AppSetting { get; } = appOptions?.Value;

    public Task<IEnumerable<AccumulateWinloss>> GetUserAccumulateWinlossAsync(string custId, DateTime date)
        => dynamicRepository.FetchAsync<AccumulateWinloss>(new GetUserAccumulateWinlossCriteria(!AppSetting.IsMainService)
        {
            CurrentDate = date,
            CustIds = custId
        });


    public Task<IEnumerable<AccumulateWinloss>> GetLastestAccumulateWinlossAsync(DateTime lastCheckTime, int batchCount)
        => dynamicRepository.FetchAsync<AccumulateWinloss>(new GetLastestAccumulateCriteria(!AppSetting.IsMainService)
        {
            BatchNo = batchCount,
            LastCheckTIme = lastCheckTime
        });
}