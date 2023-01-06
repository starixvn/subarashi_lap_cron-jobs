using Fanex.Data.Repository;
using System;

namespace Fanex.Athena.CronJobs.Infrastructure.Criteria.Command
{
    public class SystemSettingCommand : NonQueryCommand
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public override string GetSettingKey() => "Lrf_SystemSettingsUpd";

        public override bool IsValid() => !string.IsNullOrEmpty(Name);
    }
}