using System;
using System.Configuration;

namespace SmartConfig.Data
{
    public abstract class AppConfigSectionHandler
    {
        protected AppConfigSectionHandler(Type sectionType)
        {
            SectionType = sectionType;
        }

        public Type SectionType { get; private set; }

        public abstract string Select(ConfigurationSection section, string key);

        public abstract  void Update(ConfigurationSection section, string key, string value);
    }
}
