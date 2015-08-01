using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class AppConfig : IDataSource
    {
        // https://regex101.com/r/vA9kR5/3
        //private static readonly string SectionNamePattern = @"^(?:[A-Z0-9_]+\.)?(?:(?<SectionName>ConnectionStrings|AppSettings)\.)";

        private static class SectionNames
        {
            public const string ConnectionStrings = "ConnectionStrings";
            public const string AppSettings = "AppSettings";
        }

        public string Select(IDictionary<string, string> keys)
        {
            var name = keys["Name"];
            var sectionName = name.Split('.').First();

            // Experiments.
            //var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var actualSectionName = exeConfig.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            //var section = exeConfig.Sections[actualSectionName] as NameValueCollection;
            //var section = ConfigurationManager.GetSection(actualSectionName) as NameValueCollection;

            name = name.Substring(name.IndexOf('.') + 1);

            string value = null;
            switch (sectionName)
            {
            case SectionNames.ConnectionStrings:
                value = ConfigurationManager.ConnectionStrings[name].ConnectionString;
                break;
            case SectionNames.AppSettings:
                //var actualName = ConfigurationManager.AppSettings.Keys.Cast<string>().SingleOrDefault(k => k.Equals(name, StringComparison.OrdinalIgnoreCase));
                value = ConfigurationManager.AppSettings[name];
                break;
            }

            return value;
        }

        public void Update(IDictionary<string, string> keys, string value)
        {
            //ConfigurationManager.AppSettings[configElement.Name] = configElement.Value;
            //throw new NotSupportedException("AppConfig data source does not support updating (yet).");
        }


    }
}
