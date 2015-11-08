using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    public class MemberNotFoundException : SmartException
    {
        public string MemberName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
