using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class AppConfig : DataSourceBase
    {
        // https://regex101.com/r/vA9kR5/3
        private static readonly string SectionNamePattern = @"^(?:[A-Z0-9_]+\.)?(?:(?<SectionName>ConnectionStrings|AppSettings)\.)";

        private class SectionNames
        {
            public const string ConnectionStrings = "ConnectionStrings";
            public const string AppSettings = "AppSettings";
        }

        public override string Select(IDictionary<string, string> keys)
        {
            var name = keys["Name"];
            var sectionNameMatch = Regex.Match(name, SectionNamePattern, RegexOptions.IgnoreCase);
            if (!sectionNameMatch.Success)
            {
                // TODO: Create an exception for this.
                throw new Exception("Config section not found.");
            }

            // Remove the section name:
            name = Regex.Replace(name, SectionNamePattern, string.Empty, RegexOptions.IgnoreCase);

            string value = null;

            switch (sectionNameMatch.Groups["SectionName"].Value)
            {
            case SectionNames.ConnectionStrings:
                value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                break;
            case SectionNames.AppSettings:
                name = ConfigurationManager.AppSettings.Keys.Cast<string>().SingleOrDefault(k => k.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                value = ConfigurationManager.AppSettings[name];
                break;
            }

            return value;
        }

        public override void Update(IDictionary<string, string> keys, string value)
        {
            //ConfigurationManager.AppSettings[configElement.Name] = configElement.Value;
            //throw new NotSupportedException("AppConfig data source does not support updating (yet).");
        }


    }
}
