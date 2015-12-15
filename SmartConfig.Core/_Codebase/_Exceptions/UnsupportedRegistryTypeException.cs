using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class UnsupportedRegistryTypeException : SmartException
    {
        public string ValueTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }
}
