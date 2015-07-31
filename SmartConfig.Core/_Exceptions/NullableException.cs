using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class NullableException : Exception
    {
        public NullableException(Type configType, string name)
            : base(string.Format("[{0}]'s field [{1}] is not nullable. If you want it to be nuallable add the NullableAttribute.", configType.Name, name))
        {
            ConfigType = configType;
            Name = name;
        }

        public Type ConfigType { get; private set; }

        public string Name { get; private set; }
    }
}
