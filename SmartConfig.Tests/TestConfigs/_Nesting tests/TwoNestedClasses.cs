using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class TwoNestedClasses
    {
        public static string StringField;

        public class SubClass
        {
            public class SubSubClass
            {
                public static string StringField;
            }
        }
    }
}
