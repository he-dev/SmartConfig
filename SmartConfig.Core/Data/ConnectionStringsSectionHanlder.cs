using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class ConnectionStringsSectionHanlder:AppConfigSectionHanlder
    {
        public override Type SectionType { get { return typeof(ConnectionStringsSection); } }

        public override string Select(ConfigurationSection section, string key)
        {
            var connectionStrings = (section as ConnectionStringsSection);
            var actualKey = GetActualKey(connectionStrings, key);
            return string.IsNullOrEmpty(actualKey) ? null : connectionStrings.ConnectionStrings[key].ConnectionString;
        }

        public override void Update(ConfigurationSection section, string key, string value)
        {
            var connectionStrings = (section as ConnectionStringsSection);
            var actualKey = GetActualKey(connectionStrings, key);
            if (!string.IsNullOrEmpty(actualKey))
            {
                connectionStrings.ConnectionStrings.Remove(actualKey);
            }
            connectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(key, value));
        }

        private static string GetActualKey(ConnectionStringsSection section, string key)
        {
            var connectionStringSettings = section.ConnectionStrings.Cast<ConnectionStringSettings>().SingleOrDefault(k => k.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            return connectionStringSettings == null ? null : connectionStringSettings.Name;
        }
    }
}
