using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartConfig.Data
{
    internal delegate string GetStringMethod();
    internal delegate void SetStringMethod(string value);

    /// <summary>
    /// Basic setting class. Custom setting must be derived from this type.
    /// </summary>
    public class Setting : IIndexer
    {
        private const string getPrefix = "get_";
        private const string setPrefix = "set_";

        private readonly IDictionary<string, Delegate> _delegates;

        /// <summary>
        /// Creates a new setting.
        /// </summary>
        public Setting()
        {
            _delegates = new Dictionary<string, Delegate>();

            InitializeDelegates();
        }

        /// <summary>
        /// Gets or sets the specified property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get { return ((GetStringMethod)_delegates[$"{getPrefix}{propertyName}"])(); }
            set { ((SetStringMethod)_delegates[$"{setPrefix}{propertyName}"])(value); }
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
                var getStringMethod = (GetStringMethod)Delegate.CreateDelegate(typeof(GetStringMethod), this, property.GetGetMethod());
                var setStringMethod = (SetStringMethod)Delegate.CreateDelegate(typeof(SetStringMethod), this, property.GetSetMethod());
                _delegates.Add($"{getPrefix}{property.Name}", getStringMethod);
                _delegates.Add($"{setPrefix}{property.Name}", setStringMethod);
            }
        }
    }
}
