using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public class NullableValueTypeFields
    {
        public static bool? NullableBooleanField;
        public static char? NullableCharField;
        public static short? NullableInt16Field;
        public static int? NullableInt32Field;
        public static long? NullableInt64Field;
        public static float? NullableSingleField;
        public static double? NullableDoubleField;
        public static decimal? NullableDecimalField;
    }
}
