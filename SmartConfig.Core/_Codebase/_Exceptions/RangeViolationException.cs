using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class RangeViolationException : SmartException
    {
        public string Value { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Range { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
