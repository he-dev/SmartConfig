using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal class ConfigFieldInfo
    {
        public Type ConfigType;
        public IDictionary<string, string> Keys;
        public string ElementName;
        public IEnumerable<ConstraintAttribute> ElementConstraints;
    }
}
