namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command;

using Data.Repository;

public class SystemSettingCommand : NonQueryCommand
{
    public string Name { get; set; }

    public string Value { get; set; }

    public override string GetSettingKey() => "Lrf_SystemSettingsUpd";

    public override bool IsValid() => !string.IsNullOrEmpty(Name);
}