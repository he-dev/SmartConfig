using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.DataAnnotations;
using SmartConfig.Filters;

namespace SmartConfig.Data
{
    internal delegate string StringPropertyGetter();
    internal delegate void StringPropertySetter(string value);

    /// <summary>
    /// Basic setting class. Custom setting must be derived from this type.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BasicSetting : IIndexable
    {
        private IReadOnlyCollection<PropertyInfo> _filterProperties;

        private const string GetterPrefix = "get_";
        private const string SetterPrefix = "set_";

        public const string MainKeyName = nameof(Name);

        private readonly IDictionary<string, StringPropertyGetter> _getters = new Dictionary<string, StringPropertyGetter>();
        private readonly IDictionary<string, StringPropertySetter> _setters = new Dictionary<string, StringPropertySetter>();

        /// <summary>
        /// Creates a new setting.
        /// </summary>
        public BasicSetting()
        {
            InitializeFilterProperties();
            InitializeFilterDelegates();
        }

        /// <summary>
        /// Gets or sets the specified property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get
            {
                var stringPropertyGetter = (StringPropertyGetter)null;
                if (_getters.TryGetValue($"{GetterPrefix}{propertyName}", out stringPropertyGetter))
                {
                    return stringPropertyGetter();
                }
                throw new InvalidPropertyNameException
                {
                    PropertyName = propertyName,
                    SettingType = GetType().Name,
                };
            }
            set
            {
                if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException(nameof(propertyName)); }

                var stringPropertySetter = (StringPropertySetter)null;
                if (_setters.TryGetValue($"{SetterPrefix}{propertyName}", out stringPropertySetter))
                {
                    _setters[$"{SetterPrefix}{propertyName}"](value);
                    return;
                }
                throw new InvalidPropertyNameException
                {
                    PropertyName = propertyName,
                    SettingType = GetType().FullName
                };
            }
        }

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting.
        /// </summary>
        public string Value { get; set; }

        public bool IsCustomSetting => GetType() != typeof(BasicSetting);

        public IReadOnlyCollection<string> CustomKeyNames => _filterProperties.Select(x => x.Name).ToList();

        public IReadOnlyDictionary<string, ISettingFilter> GetCustomKeyFilters()
        {
            var customSettingFilters = _filterProperties.Select(p => new
            {
                p.Name,
                p.GetCustomAttribute<SettingFilterAttribute>().FilterType
            })
            .ToDictionary(x => x.Name, x => (ISettingFilter)Activator.CreateInstance(x.FilterType));
            return new ReadOnlyDictionary<string, ISettingFilter>(customSettingFilters);
        }

        private void InitializeFilterProperties()
        {
            if (!IsCustomSetting)
            {
                _filterProperties = new List<PropertyInfo>();
                return;
            }

            _filterProperties =
                GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.HasAttribute<SettingFilterAttribute>())
                .OrderBy(p => p.Name).ToList();
        }

        private void InitializeFilterDelegates()
        {
            foreach (var property in _filterProperties)
            {
                var getStringMethod = (StringPropertyGetter)Delegate.CreateDelegate(typeof(StringPropertyGetter), this, property.GetGetMethod());
                var setStringMethod = (StringPropertySetter)Delegate.CreateDelegate(typeof(StringPropertySetter), this, property.GetSetMethod());
                _getters.Add($"{GetterPrefix}{property.Name}", getStringMethod);
                _setters.Add($"{SetterPrefix}{property.Name}", setStringMethod);
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Join(" ",
                    new[] { $"Name = '{Name}'" }
                    .Concat(_filterProperties.Select(c => $"{c.Name} = '{this[c.Name]}'"))
                    .Concat(new[] { $"Value = '{Value}'" })
                );

            }
        }
    }
}
