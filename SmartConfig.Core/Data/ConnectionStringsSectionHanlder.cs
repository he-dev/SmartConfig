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
            return (section as ConnectionStringsSection).ConnectionStrings[key].ConnectionString;
        }

        public override void Update(ConfigurationSection section, string key, string value)
        {
            (section as ConnectionStringsSection).ConnectionStrings[key].ConnectionString = value;
        }
    }
}
