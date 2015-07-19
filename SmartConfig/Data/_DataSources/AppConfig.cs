using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class AppConfig : DataSource
    {
        public override IEnumerable<ConfigElement> Select(string key)
        {
            var appConfigKey = ConfigurationManager.AppSettings.Keys.Cast<string>().Where(k => k.Equals(key, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            return new List<ConfigElement>(){ new ConfigElement()
            {
                Name = key,
                Data = ConfigurationManager.AppSettings[appConfigKey]
            }};
        }

        public override void Update(ConfigElement configElement)
        {
            throw new NotSupportedException("AppConfig data source does not support updating.");
        }
    }
}
