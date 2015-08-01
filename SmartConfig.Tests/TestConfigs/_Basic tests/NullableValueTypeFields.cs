using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;
using SmartConfig.Data;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class NullableValueTypeFields
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

    public class NullableValuesTypeFieldsDataSource : IDataSource
    {
        public string Select(IDictionary<string, string> keys)
        {
            throw new NotImplementedException();
        }

        public void Update(IDictionary<string, string> keys, string value)
        {
            throw new NotImplementedException();
        }
    }
}
