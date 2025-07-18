namespace Fanex.Athena.CronJobs.Infrastructure.Repository;

using System.Collections.Generic;
using System.Threading.Tasks;
using Criteria.Command;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Params;
using Domain.Repository;

public class BellNotificationRepository(IDynamicRepository dynamicRepository) : IBellNotificationRepository
{
    public Task InsertNotificationAsync(InsertBellNotificationParam param)
    {
        return dynamicRepository.ExecuteAsync(new BellNotificationCommand
        {
            MessageType = param.MessageType,
            LicenseeNames = param.LicenseeNames,
            SubjectEn = param.SubjectEn,
            SubjectCs = param.SubjectCs,
            ContentEn = param.ContentEn,
            ContentCs = param.ContentCs,
            EventDate = param.EventDate
        });
    }

    public Task InsertMPANotificationAsync(InsertBellNotificationParam param)
    {
        return dynamicRepository.ExecuteAsync(new MPABellNotificationCommand
        {
            MessageType = param.MessageType,
            LicenseeNames = param.LicenseeNames,
            SubjectEn = param.SubjectEn,
            SubjectCs = param.SubjectCs,
            ContentEn = param.ContentEn,
            ContentCs = param.ContentCs,
            EventDate = param.EventDate
        });
    }

    public Task<int> GetDWHDailyJobStatusAsync(int jobId)
    {
        return dynamicRepository.GetAsync<int>(new DWHDailyJobStatusCriteria
        {
            TaskIDList = jobId.ToString()
        });
    }

    public Task<IEnumerable<AbnormalBet>> GetDailyAbnormalBetAsync()
    {
        return dynamicRepository.FetchAsync<AbnormalBet>(new GetAbnormalBetCriteria());
    }

    public Task<SystemSetting> GetSystemSettingAsync(string name)
    {
        return dynamicRepository.GetAsync<SystemSetting>(new GetSystemSettingCriteria
        {
            Name = name
        });
    }

    public Task UpdateSystemSettingAsync(SystemSetting setting)
    {
        return dynamicRepository.ExecuteAsync(new SystemSettingCommand
        {
            Name = setting.Name,
            Value = setting.Value
        });
    }
}