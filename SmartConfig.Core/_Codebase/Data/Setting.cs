using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public class Setting : Dictionary<string, object>
    {
        public Setting() : base(StringComparer.OrdinalIgnoreCase) { }

        public SettingPath Name
        {
            [DebuggerStepThrough]
            get { return (SettingPath)this[nameof(Name)]; }

            [DebuggerStepThrough]
            set { this[nameof(Name)] = value; }
        }

        public object Value
        {
            [DebuggerStepThrough]
            get { return this[nameof(Value)]; }

            [DebuggerStepThrough]
            set { this[nameof(Value)] = value; }
        }

        public string ConfigName
        {
            [DebuggerStepThrough]
            get { return (string)this[nameof(ConfigName)]; }

            [DebuggerStepThrough]
            set { this[nameof(ConfigName)] = value; }
        }

        public bool NamespaceEquals(string key, object value)
        {
            var temp = (object)null;
            return TryGetValue(key, out temp) && temp.Equals(value);
        }
    }
}
