using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class InvalidMemberTypeException : SmartException
    {
        public string MemberName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string DeclaringTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string MemberTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string ExpectedTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
