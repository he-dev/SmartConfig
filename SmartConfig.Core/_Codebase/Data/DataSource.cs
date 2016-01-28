using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Filters;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements and extends the <c>IDataSource</c> interface.
    /// </summary>
    public abstract class DataSource<TSetting> : IDataSource where TSetting : Setting, new()
    {
        protected DataSource()
        {
            const BindingFlags customPropertiesBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            CustomKeyFilters =
                typeof(TSetting) == typeof(Setting)
                ? new Dictionary<string, ISettingFilter>()
                : typeof(TSetting).GetProperties(customPropertiesBindingFlags).ToDictionary(x => x.Name, x =>
                {
                    var filterAttribute = x.GetCustomAttribute<FilterAttribute>();
                    if (filterAttribute == null)
                    {
                        throw new ArgumentNullException();
                    }
                    var settingFilter = (ISettingFilter)Activator.CreateInstance(filterAttribute.FilterType);
                    return settingFilter;
                });

            //var isDerived = typeof(TSetting) != typeof(Setting);
            //if (isDerived)
            //{
            //    if (customKeys == null)
            //    {
            //        throw new ArgumentNullException(nameof(customKeys));
            //    }
            //    var customKeyNames = SettingKeyNames.Where(kn => kn != SettingKeyNames.DefaultKeyName).ToList();
            //    var allKeysConfigured = customKeys.All(ck => ck.Filter != null && customKeyNames.Contains(ck.Name));
            //    if (!allKeysConfigured)
            //    {
            //        throw new SettingCustomKeysMismatchException
            //        {
            //            CustomKeys = string.Join(" ", customKeys.Select(ck => $"{ck.Name} = \"{ck.Value}\""))
            //        };
            //    }
            //    _customKeys = customKeys.ToDictionary(ck => ck.Name);
            //}
        }

        protected IDictionary<string, ISettingFilter> CustomKeyFilters { get; }

        public abstract IReadOnlyCollection<Type> SupportedTypes { get; }

        public abstract object Select(SettingKeyCollection keys);

        public abstract void Update(SettingKeyCollection keys, object value);

        /// <summary>
        /// Applies all of the specified filters.
        /// </summary>
        protected IEnumerable<TSetting> ApplyFilters(IEnumerable<TSetting> settings, IEnumerable<SettingKey> customKeys)
        {
            settings = customKeys.Aggregate(settings, (current, key) => CustomKeyFilters[key.Name].FilterSettings(current, key).Cast<TSetting>());
            return settings;
        }
    }
}
