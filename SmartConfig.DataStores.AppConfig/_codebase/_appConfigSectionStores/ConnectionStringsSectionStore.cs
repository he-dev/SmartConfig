using System;
using System.Configuration;
using SmartConfig.Data;

namespace SmartConfig.DataStores.AppConfig
{
    public class ConnectionStringsSectionStore : AppConfigSectionStore<ConnectionStringsSection>, IAppConfigSectionStore
    {
        public ConnectionStringsSectionStore(ConnectionStringsSection connectionStringsSection) : base(connectionStringsSection)
        {

        }

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
