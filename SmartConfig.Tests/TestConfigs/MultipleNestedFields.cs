using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class MultipleNestedFields
    {
        public class SubConfig
        {
            public class SubSubConfig
            {
                public static string StringField;
            }
        }
    }
}
