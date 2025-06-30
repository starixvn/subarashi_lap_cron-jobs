namespace Fanex.Athena.CronJobs.Infrastructure.Repository;

using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Application.Settings;
using Criteria.Command;
using Criteria.Query;
using Data.Repository;
using Domain.DTO;
using Domain.Params;
using Domain.Repository;
using Microsoft.Extensions.Options;

public class NotificationRepository(IDynamicRepository dynamicRepository, IOptions<AppSetting> appOptions) : INotificationRepository
{
    private AppSetting AppSetting { get; } = appOptions?.Value;

    public Task InsertNotificationAsync(InsertNotificationParam param)
    {
        var xmlReachCust = BuildReachCust(param.Customers);

        return dynamicRepository.ExecuteAsync(new NotificationCommand(!AppSetting.IsMainService)
        {
            CustReached = xmlReachCust,
            LastCheckTime = param.LastCheckTime,
            LastScanSettingDate = param.LastScanSettingDate,
            NotificationType = param.NotificationType,
            ServiceName = param.ServiceName
        });
    }

    public Task<IEnumerable<CustThresholdSetting>> GetCustThresholdSettingAsync(GetSettingThresholdParam param)
        => dynamicRepository.FetchAsync<CustThresholdSetting>(new GetCustThresholdSettingCriteria
        {
            CustList = param.CustList,
            IsDeposit = param.IsDeposit,
            SportType = param.SportType,
            IsTest = param.IsTest,
            SuperList = param.SuperList
        });

    public Task<IEnumerable<CustThresholdSetting>> GetCustChangeThresholdSettingAsync(ChangeThresholdSettingParam param)
        => dynamicRepository.FetchAsync<CustThresholdSetting>(new GetChangeThresholdSettingCriteria
        {
            LastScanDate = param.LastScanDate,
            SuperList = param.SuperList,
            SportType = param.SportType,
            IsTest = param.IsTest,
            IsDeposit = param.IsDeposit
        });

    private static string BuildReachCust(IEnumerable<CustAccumulateParam> customers)
    {
        var xmlDoc = new XmlDocument();
        var root = xmlDoc.CreateElement("CustIds");
        xmlDoc.AppendChild(root);
        if (customers is not null)
        {
            foreach (var cust in customers)
            {
                var node = xmlDoc.CreateElement("Cust");
                node.SetAttribute("CustId", cust.CustId.ToString());
                node.SetAttribute("AccumWL", cust.AccumWL.ToString(CultureInfo.InvariantCulture));
                node.SetAttribute("SportType", cust.SportType.ToString());
                node.SetAttribute("WinningThreshold", cust.WinningThreshold.ToString(CultureInfo.InvariantCulture));

                root.AppendChild(node);
            }
        }

        return xmlDoc.OuterXml;
    }
}