using System.Text;

namespace SmartConfig.DataStores.SQLite
{
    internal static class StringEncoder
    {
        public static string Recode(this string value, Encoding from, Encoding to)
        {
            return to.GetString(from.GetBytes(value));
        }
    }
}
