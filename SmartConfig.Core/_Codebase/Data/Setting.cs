using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartConfig.Data
{
    public delegate string GetStringDelegate();
    public delegate void SetStringDelegate(string value);

    /// <summary>
    /// Basic setting class. Custom setting must be derived from this type.
    /// </summary>
    public class Setting : IIndexer
    {
        private readonly IDictionary<string, GetStringDelegate> _getStringDelegates;
        private readonly IDictionary<string, SetStringDelegate> _setStringDelegates;

        /// <summary>
        /// Creates a new setting.
        /// </summary>
        public Setting()
        {
            _getStringDelegates = new Dictionary<string, GetStringDelegate>();
            _setStringDelegates = new Dictionary<string, SetStringDelegate>();

            var isDerived = GetType() != typeof(Setting);
            if (isDerived)
            {
                var properties = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    _getStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(GetStringDelegate), this, property.GetGetMethod()) as GetStringDelegate);
                    _setStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(SetStringDelegate), this, property.GetSetMethod()) as SetStringDelegate);
                }
            }
        }

        /// <summary>
        /// Gets or sets the specified property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string this[string propertyName]
        {
            get { return _getStringDelegates[propertyName](); }
            set { _setStringDelegates[propertyName](value); }
        }

        /// <summary>
        /// Gets or sets the name of the setting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the setting.
        /// </summary>
        public string Value { get; set; }
    }
}
