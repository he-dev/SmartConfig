using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class RootFields
    {
        public static char CharField;
        public static string StringField;
        public static short Int16Field;
        public static short? NullableInt16Field;
        public static int Int32Field;
        public static int? NullableInt32Field;
        public static long Int64Field;
        public static long? NullableInt64Field;

        public static TestEnum EnumField;

        [ObjectConverter(typeof(JsonConverter))]
        public static List<Int32> ListInt32Field;
    }

    public enum TestEnum
    {
        TestValue1,
        TestValue2
    }
}
