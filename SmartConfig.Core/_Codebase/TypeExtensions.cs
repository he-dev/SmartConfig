using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal static class TypeExtensions
    {
        public static bool IsDictionary(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        public static bool IsList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        public static bool IsHashSet(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>);
    }
}
