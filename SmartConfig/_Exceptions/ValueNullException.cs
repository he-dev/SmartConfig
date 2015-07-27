using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class ValueNullException : Exception
    {
        public ValueNullException(Type configType, string elementName)
            : base("message")
        {
            ConfigType = configType;
            ElementName = elementName;
        }

        public Type ConfigType { get; private set; }

        public string ElementName { get; private set; }
    }
}
