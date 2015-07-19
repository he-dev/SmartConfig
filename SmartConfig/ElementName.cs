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
    internal static class ElementName
    {
        public static string From<T>(Expression<Func<T>> expression)
        {
            var memberInfo = SmartConfig.GetMemberInfo(expression);
            var smartConfigType = SmartConfig.GetSmartConfigType(memberInfo);

            // Replace config namespace and class name with application name.
            var elementName = Regex.Replace(memberInfo.ReflectedType.FullName, @"^" + smartConfigType.FullName + @"\.?", string.Empty);

            // Replace subclass separator "+" with a "."
            elementName = Regex.Replace(elementName, @"\+", ".");

            // Add member name.
            elementName = Combine(elementName, memberInfo.Name);

            // Add application name if available.
            var configName = Combine(smartConfigType.ConfigName(), elementName);

            // Remove invalid "." at the beginning. It's easier and cleaner to remove it here then to prevent it above.
            elementName = Regex.Replace(elementName, @"^\.", string.Empty);

            return elementName;
        }

        public static string Combine(params string[] names)
        {
            return string.Join(".", names.Where(name => !string.IsNullOrEmpty(name)));
        }        
    }
}
