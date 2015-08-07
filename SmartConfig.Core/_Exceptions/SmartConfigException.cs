using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class SmartConfigException : Exception
    {
        public SmartConfigException(ConfigFieldInfo configFieldInfo, Exception innerException)
            : base(string.Empty, innerException)
        {
            ConfigFieldInfo = configFieldInfo;
        }

        public ConfigFieldInfo ConfigFieldInfo { get; private set; }        
    }
}
