namespace Fanex.Athena.CronJobs.Infrastructure.UnitTests.Repository;

using Application.Settings;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Infrastructure.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

public class AccumulateRepositoryTests
{
    private IOptions<AppSetting> _appSetting;

    private IDynamicRepository _dynamicRepository;

    private AccumulateRepository repo;

    [SetUp]
    public void SetUp()
    {
        _dynamicRepository = Substitute.For<IDynamicRepository>();
        _appSetting = Substitute.For<IOptions<AppSetting>>();
        _appSetting.Value.Returns(new AppSetting());
        repo = new AccumulateRepository(_dynamicRepository, _appSetting);
    }

    [Test]
    public async Task GetUserAccumulateWinlossAsync_WhenCall_ReceiveCorrectParam()
    {
        //Arrange
        var custId = "1";
        var date = DateTime.Now;

        //Action
        await repo.GetUserAccumulateWinlossAsync(custId, date);

        //Assert
        await _dynamicRepository.Received()
            .FetchAsync<AccumulateWinloss>(Arg.Is<GetUserAccumulateWinlossCriteria>(_ => _.CustIds == custId && _.CurrentDate == date));
    }

    [Test]
    public async Task GetLastestAccumulateWinlossAsync_WhenCall_ReceiveCorrectParam()
    {
        //Arrange
        var date = DateTime.Now;
        var batchCount = 2000;

        //Action
        await repo.GetLastestAccumulateWinlossAsync(date, batchCount);

        //Assert
        await _dynamicRepository.Received()
            .FetchAsync<AccumulateWinloss>(Arg.Is<GetLastestAccumulateCriteria>(_ => _.LastCheckTIme == date && _.BatchNo == batchCount));
    }
}