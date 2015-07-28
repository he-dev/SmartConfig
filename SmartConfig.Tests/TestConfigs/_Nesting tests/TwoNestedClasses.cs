using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class TwoNestedClasses
    {
        public static string StringField;

        public static class SubClass
        {
            public static class SubSubClass
            {
                public static string StringField;
            }
        }
    }
}
