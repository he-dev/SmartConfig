using System;
using System.Configuration;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace SmartConfig.Data
{
    public abstract class AppConfigSectionSource<TConfigurationSection> where TConfigurationSection : ConfigurationSection
    {
        protected AppConfigSectionSource(TConfigurationSection configurationSection)
        {
            ConfigurationSection = configurationSection;
        }

        public TConfigurationSection ConfigurationSection { get; protected set; }        

        public abstract string Select(string key);

        public abstract  void Update(string key, string value);
    }
}
