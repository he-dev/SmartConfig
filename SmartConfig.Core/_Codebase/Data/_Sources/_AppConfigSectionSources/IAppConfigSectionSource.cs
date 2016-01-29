using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public interface IAppConfigSectionSource
    {
        string SectionName { get; }

        string Select(string key);

        void Update(string key, string value);
    }
}
