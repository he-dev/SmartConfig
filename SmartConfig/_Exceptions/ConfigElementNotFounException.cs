using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class ConfigElementNotFounException : Exception
    {
        public ConfigElementNotFounException(Type configType, string elementName)
            : base(string.Format("Config element [{0}] not found in [{1}]. If you want this field to be optional add the OptionalAttribute.", elementName, configType.Name))
        {
            ConfigType = configType;
            ElementName = elementName;
        }

        public Type ConfigType { get; private set; }

        public string ElementName { get; private set; }
    }
}
