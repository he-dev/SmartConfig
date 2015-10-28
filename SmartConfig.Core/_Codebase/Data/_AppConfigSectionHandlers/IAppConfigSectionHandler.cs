using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public interface IAppConfigSectionHandler
    {
        Type SectionType { get; }

        string SectionName { get; }

        string Select(ConfigurationSection section, string key);

        void Update(ConfigurationSection section, string key, string value);
    }
}
