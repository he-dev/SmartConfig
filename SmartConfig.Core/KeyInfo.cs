using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class KeyInfo<TConfigElement> where TConfigElement : ConfigElement, new()
    {
        public string KeyName { get; set; }

        public string KeyValue { get; set; }

        public FilterByFunc<TConfigElement> Filter { get; set; }
    }
}
