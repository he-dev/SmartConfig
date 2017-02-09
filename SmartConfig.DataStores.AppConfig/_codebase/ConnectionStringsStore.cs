using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    public class ConnectionStringsStore : DataStore
    {
        private readonly System.Configuration.Configuration _exeConfiguration;
        private readonly ConnectionStringsSection _connectionStringsSection;

        public ConnectionStringsStore() : base(new[] { typeof(string) })
        {
            _exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _connectionStringsSection = _exeConfiguration.ConnectionStrings;
        }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            var connectionStringSettings =
                _connectionStringsSection
                .ConnectionStrings
                .Cast<ConnectionStringSettings>()
                .Like(x => x.Name, setting);

            foreach (var x in connectionStringSettings)
            {
                yield return new Setting
                {
                    Name = SettingPath.Parse(x.Name),
                    Value = x.ConnectionString
                };
            }
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {
            foreach (var group in settings)
            {
                DeleteObsoleteSettings(group);

                foreach (var setting in group)
                {
                    var connectionStringSettings = new ConnectionStringSettings(setting.Name.StrongFullName, (string)setting.Value);
                    _connectionStringsSection.ConnectionStrings.Add(connectionStringSettings);
                }
            }
            _exeConfiguration.Save(ConfigurationSaveMode.Minimal);
        }

        private void DeleteObsoleteSettings(IGrouping<Setting, Setting> settings)
        {
            var obsoleteNames =
                _connectionStringsSection
                .ConnectionStrings
                .Cast<ConnectionStringSettings>()
                .Where(x => SettingPath.Parse(x.Name).IsLike(settings.Key.Name))
                .ToList();

            obsoleteNames.ForEach(x => _connectionStringsSection.ConnectionStrings.Remove(x.Name));
        }
    }
}
