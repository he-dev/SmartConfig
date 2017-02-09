using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Setting> Like(this IEnumerable<Setting> settings, Setting setting)
        {
            return settings.Where(x => x.Name.IsLike(setting.Name));
        }

        public static IEnumerable<string> Like(this IEnumerable<string> names, Setting setting)
        {
            return names.Where(k => SettingPath.Parse(k).IsLike(setting.Name));
        }

        public static IEnumerable<T> Like<T>(this IEnumerable<T> values, Func<T, string> getName, Setting setting)
        {
            return values.Where(x => SettingPath.Parse(getName(x)).IsLike(setting.Name));
        }
    }
}
