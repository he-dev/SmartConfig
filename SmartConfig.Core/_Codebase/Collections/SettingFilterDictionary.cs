using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartConfig.Filters;

namespace SmartConfig.Collections
{
    public class SettingFilterDictionary : ReadOnlyDictionary<string, ISettingFilter>
    {
        public SettingFilterDictionary(IDictionary<string, ISettingFilter> dictionary) : base(dictionary) { }

        public static SettingFilterDictionary Create<TSetting>() where TSetting : BasicSetting
        {
            var filters = new Dictionary<string, ISettingFilter>();

            var isBasicSetting = typeof(TSetting) == typeof(BasicSetting);
            if (isBasicSetting)
            {
                return new SettingFilterDictionary(filters);
            }

            const BindingFlags customPropertiesBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var customProperties = typeof(TSetting).GetProperties(customPropertiesBindingFlags);
            foreach (var customProperty in customProperties)
            {
                var filterAttribute = customProperty.GetCustomAttribute<SettingFilterAttribute>();
                if (filterAttribute == null)
                {
                    throw new FilterAttributeMissingException
                    {
                        DeclaringTypeName = typeof(TSetting).FullName,
                        PropertyName = customProperty.Name,
                    };
                }
                var filter = (ISettingFilter)Activator.CreateInstance(filterAttribute.FilterType);
                filters.Add(customProperty.Name, filter);
            }

            return new SettingFilterDictionary(filters);
        }
    }
}
