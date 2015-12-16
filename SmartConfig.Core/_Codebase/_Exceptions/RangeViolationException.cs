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
        public string RangeTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Min { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Max { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
