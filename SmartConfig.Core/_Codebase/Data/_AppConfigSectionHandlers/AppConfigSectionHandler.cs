using System;
using System.Configuration;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace SmartConfig.Data
{
    public abstract class AppConfigSectionHandler<T> where T : ConfigurationSection
    {
        public Type SectionType => typeof (T);

        public string SectionName => Regex.Replace(typeof (T).Name, @"Section$", string.Empty);

        public abstract string Select(ConfigurationSection section, string key);

        public abstract  void Update(ConfigurationSection section, string key, string value);
    }
}
