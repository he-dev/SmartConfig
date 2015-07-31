using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating config element names.
    /// </summary>
    internal static class ConfigElementName
    {
        public static string From<T>(Expression<Func<T>> expression)
        {
            var memberInfo = Utilities.GetMemberInfo(expression);
            var smartConfigType = Utilities.GetConfigType(memberInfo);

            // Replace config namespace and class name with application name.
            var elementName = Regex.Replace(memberInfo.ReflectedType.FullName, @"^" + smartConfigType.FullName + @"\.?", string.Empty);

            // Replace subclass separator "+" with a "."
            elementName = Regex.Replace(elementName, @"\+", ".");

            // Add member name.
            elementName = Combine(new[] { elementName, memberInfo.Name });

            // Add application name if available.
            var configName = Combine(new[] { Utilities.ConfigName(smartConfigType), elementName });

            // Remove invalid "." at the beginning. It's easier and cleaner to remove it here then to prevent it above.
            //elementName = Regex.Replace(elementName, @"^\.", string.Empty);

            return configName;
        }

        public static string Combine(IEnumerable<string> path, bool reverse = false)
        {
            return string.Join(".", (reverse ? path.Reverse() : path).Where(name => !string.IsNullOrEmpty(name)));
        }
    }
}
