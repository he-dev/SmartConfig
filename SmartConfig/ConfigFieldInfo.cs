using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal class ConfigFieldInfo
    {
        public Type SmartConfigType;
        public string Version;
        public string Name;
        public IEnumerable<ValueConstraintAttribute> Constraints;
    }
}
