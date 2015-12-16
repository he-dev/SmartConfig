using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class RegularExpressionViolationException : SmartException
    {
        public string Value { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string Pattern { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string RegexOptions { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
