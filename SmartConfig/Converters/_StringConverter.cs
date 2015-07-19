using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class StringConverter<T>
    {
        public Func<T, CultureInfo, string> ConvertFrom { get; set; }
        public Func<string, CultureInfo, T> ConvertTo { get; set; }
    }

    
}
