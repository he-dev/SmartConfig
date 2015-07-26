using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class EnumFields
    {
        public static TestEnum EnumField1;
        //public static TestEnum? EnumField2;
    }
}
