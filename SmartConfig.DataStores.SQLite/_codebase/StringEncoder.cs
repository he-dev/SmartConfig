using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace SmartConfig.DataStores.SQLite
{
    public static class StringEncoder
    {
        //public static string ToUTF8(this string value)
        //{
        //    var result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));
        //    return result;
        //}

        //public static IEnumerable<string> ToUTF8(this IEnumerable<string> values)
        //{
        //    var result = values.Select(x => x.ToUTF8());
        //    return result;
        //}

        public static string Recode(this string value, Encoding from, Encoding to)
        {
            return to.GetString(from.GetBytes(value));
        }
    }
}
