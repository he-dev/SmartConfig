using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.DataStores.SQLite
{
    public static class StringEncoder
    {
        public static string AsUTF8(this string value)
        {
            var result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));
            return result;
        }

        public static IEnumerable<string> AsUTF8(this IEnumerable<string> values)
        {
            var result = values.Select(x => x.AsUTF8());
            return result;
        }
    }
}
