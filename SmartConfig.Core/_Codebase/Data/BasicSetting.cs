using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartConfig.Data
{
    internal delegate string StringPropertyGetter();
    internal delegate void StringPropertySetter(string value);

    /// <summary>
    /// Basic setting class. Custom setting must be derived from this type.
    /// </summary>
    public class BasicSetting : IIndexable
    {
        private const string GetterPrefix = "get_";
        private const string SetterPrefix = "set_";

        public const string DefaultKeyName = nameof(Name);

        private readonly IDictionary<string, StringPropertyGetter> _getters = new Dictionary<string, StringPropertyGetter>();
        private readonly IDictionary<string, StringPropertySetter> _setters = new Dictionary<string, StringPropertySetter>();

        /// <summary>
        /// Creates a new setting.
        /// </summary>
        public BasicSetting()
        {
            InitializeDelegates();
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
                StringPropertyGetter stringPropertyGetter;
                if (!_getters.TryGetValue($"{GetterPrefix}{propertyName}", out stringPropertyGetter))
                {
                    throw new InvalidPropertyNameException
                    {
                        PropertyName = propertyName,
                        TargetType = GetType().FullName
                    };
                }
                return stringPropertyGetter();
            }
            set
            {
                StringPropertySetter stringPropertySetter;
                if (!_setters.TryGetValue($"{SetterPrefix}{propertyName}", out stringPropertySetter))
                {
                    throw new InvalidPropertyNameException
                    {
                        PropertyName = propertyName,
                        TargetType = GetType().FullName
                    };
                }
                _setters[$"{SetterPrefix}{propertyName}"](value);
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

        private void InitializeDelegates()
        {
            var isDerived = GetType() != typeof(BasicSetting);
            if (!isDerived)
            {
                return;
            }

            var properties = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var getStringMethod = (StringPropertyGetter)Delegate.CreateDelegate(typeof(StringPropertyGetter), this, property.GetGetMethod());
                var setStringMethod = (StringPropertySetter)Delegate.CreateDelegate(typeof(StringPropertySetter), this, property.GetSetMethod());
                _getters.Add($"{GetterPrefix}{property.Name}", getStringMethod);
                _setters.Add($"{SetterPrefix}{property.Name}", setStringMethod);
            }
        }
    }
}
