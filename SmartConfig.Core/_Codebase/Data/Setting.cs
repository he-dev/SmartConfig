using System;
using System.Collections.Generic;
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
            get { return (SettingPath)this[nameof(Name)]; }
            set { this[nameof(Name)] = value; }
        }

        public object Value
        {
            get { return this[nameof(Value)]; }
            set { this[nameof(Value)] = value; }
        }
    }
}
