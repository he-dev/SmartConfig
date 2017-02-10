using System.Text;

namespace SmartConfig.DataStores.SQLite
{
    public static class StringEncoder
    {
        public static string Recode(this string value, Encoding from, Encoding to)
        {
            return to.GetString(from.GetBytes(value));
        }
    }
}
