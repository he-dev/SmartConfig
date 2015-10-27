using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    /// <summary>
    /// Provides utility methods for creating config element names.
    /// </summary>
    public class SettingPath : List<string>
    {
        //public static string From<T>(Expression<Func<T>> expression)
        //{
        //    var memberInfo = Utilities.GetMemberInfo(expression);
        //    var settingInfo = SettingInfo.From(expression);

        //    // Replace config namespace and class name with application name.
        //    var elementName = Regex.Replace(memberInfo.ReflectedType.FullName, @"^" + settingInfo.ConfigType.FullName + @"\.?", string.Empty);

        //    // Replace subclass separator "+" with a "."
        //    elementName = Regex.Replace(elementName, @"\+", ".");

        //    // Add member name.
        //    //elementName = Combine(new[] { elementName, memberInfo.Name });

        //    // Add application name if available.
        //    //var configName = Combine(new[] { settingInfo.ConfigName, elementName });

        //    // Remove invalid "." at the beginning. It's easier and cleaner to remove it here then to prevent it above.
        //    //elementName = Regex.Replace(elementName, @"^\.", string.Empty);

        //    //return configName;
        //    return String.Empty;
        //}

        public SettingPath(IEnumerable<string> names)
        {
            AddRange(names);
        }

        public SettingPath(params string[] names) : this((IEnumerable<string>)names)
        {
        }

        public static SettingPath From(IEnumerable<string> names, bool reversed)
        {
            if (reversed)
            {
                names = names.Reverse();
            }

            return new SettingPath(names);
        }

        public override string ToString()
        {
            return string.Join(".", this.Where(name => !string.IsNullOrEmpty(name)));
        }

        public static implicit operator string (SettingPath settingPath)
        {
            return settingPath.ToString();
        }
    }
}
