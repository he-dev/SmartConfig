using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class ConfigTypeNotStaticException : SmartException
    {
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
