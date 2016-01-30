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
    public class Setting : IIndexer
    {
        private const string GetPrefix = "get_";
        private const string SetPrefix = "set_";

        public const string DefaultKeyName = nameof(Name);

        private readonly IDictionary<string, StringPropertyGetter> _getters = new Dictionary<string, StringPropertyGetter>();
        private readonly IDictionary<string, StringPropertySetter> _setters = new Dictionary<string, StringPropertySetter>();

        /// <summary>
        /// Creates a new setting.
        /// </summary>
        public Setting()
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
                if (!_getters.TryGetValue($"{GetPrefix}{propertyName}", out stringPropertyGetter))
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
                if (!_setters.TryGetValue($"{GetPrefix}{propertyName}", out stringPropertySetter))
                {
                    throw new InvalidPropertyNameException
                    {
                        PropertyName = propertyName,
                        TargetType = GetType().FullName
                    };
                }
                _setters[$"{SetPrefix}{propertyName}"](value);
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
            var isDerived = GetType() != typeof(Setting);
            if (!isDerived)
            {
                return;
            }

            var properties = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var getStringMethod = (StringPropertyGetter)Delegate.CreateDelegate(typeof(StringPropertyGetter), this, property.GetGetMethod());
                var setStringMethod = (StringPropertySetter)Delegate.CreateDelegate(typeof(StringPropertySetter), this, property.GetSetMethod());
                _getters.Add($"{GetPrefix}{property.Name}", getStringMethod);
                _setters.Add($"{SetPrefix}{property.Name}", setStringMethod);
            }
        }
    }
}
