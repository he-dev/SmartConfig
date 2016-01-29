using System;
using System.Configuration;
using System.Diagnostics;

namespace SmartConfig.Data
{
    public class ConnectionStringsSectionSource : AppConfigSectionSource<ConnectionStringsSection>, IAppConfigSectionSource
    {
        public ConnectionStringsSectionSource(ConnectionStringsSection connectionStringsSection) : base(connectionStringsSection)
        {

        }

        public string SectionName => ConfigurationSection.SectionInformation.Name;

        public override string Select(string key)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            var connectionStringSettings = ConfigurationSection.ConnectionStrings[key];
            return connectionStringSettings?.ConnectionString;
        }

        public override void Update(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) { throw new ArgumentNullException(nameof(key)); }

            var connectionStringSettings = ConfigurationSection.ConnectionStrings[key];
            if (connectionStringSettings == null)
            {
                connectionStringSettings = new ConnectionStringSettings(key, value);
                ConfigurationSection.ConnectionStrings.Add(connectionStringSettings);
            }
            else
            {
                connectionStringSettings.ConnectionString = value;
            }
        }
    }
}
