using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public abstract class AppConfigSectionHanlder
    {
        public abstract Type SectionType { get; }

        public abstract string Select(ConfigurationSection section, string key);

        public abstract  void Update(ConfigurationSection section, string key, string value);
    }
}
