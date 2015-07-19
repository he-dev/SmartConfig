using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class StringParser
    {
        public static readonly Dictionary<Type, Func<string, CultureInfo, object>> Parsers = new Dictionary<Type, Func<string, CultureInfo, object>>();

        static StringParser()
        {
            Parsers[typeof(string)] = (s, ci) => s;
            Parsers[typeof(int)] = (s, ci) => (object)int.Parse(s, ci);
            Parsers[typeof(float)] = (s, ci) => (object)float.Parse(s, ci);
        }

        public static object Parse(string value, Type type, CultureInfo cultureInfo)
        {
            Func<string, CultureInfo, object> parseFunc = null;
            if (!Parsers.TryGetValue(type, out parseFunc))
            {
                throw new KeyNotFoundException(string.Format("Parser for [{0}] not found.", type.Name));
            }
            return parseFunc(value, cultureInfo);
        }
    }
}
