using System.Configuration;
using System.Diagnostics;

namespace SmartConfig.Data
{
    public class ConnectionStringsSectionHandler : AppConfigSectionHandler
    {
        public ConnectionStringsSectionHandler() : base(typeof(ConnectionStringsSection)) { }

        public override string Select(ConfigurationSection section, string key)
        {
            Debug.Assert(section is ConnectionStringsSection);

            var connectionStrings = (ConnectionStringsSection)section;
            var connectionStringSettings = connectionStrings.ConnectionStrings[key];
            return connectionStringSettings?.ConnectionString;
        }

        public override void Update(ConfigurationSection section, string key, string value)
        {
            Debug.Assert(section is ConnectionStringsSection);

            var connectionStrings = (ConnectionStringsSection)section;
            var connectionStringSettings = connectionStrings.ConnectionStrings[key];
            if (connectionStringSettings == null)
            {
                connectionStringSettings = new ConnectionStringSettings(key, value);
                connectionStrings.ConnectionStrings.Add(connectionStringSettings);
            }
            else
            {
                connectionStringSettings.ConnectionString = value;
            }
        }
    }
}
