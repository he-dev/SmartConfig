using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class OneNestedClass
    {
        public static string StringField;

        public class SubClass
        {
            public static string StringField;
        }
    }
}
