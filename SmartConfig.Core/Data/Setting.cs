using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace SmartConfig.Data
{
    public delegate string GetStringDelegate();
    public delegate void SetStringDelegate(string value);

    /// <summary>
    /// This is the default basic config element. Custom config elements must be derived from this type.
    /// </summary>
    public class Setting : IIndexer
    {
        private readonly IDictionary<string, GetStringDelegate> _getStringDelegates;
        private readonly IDictionary<string, SetStringDelegate> _setStringDelegates;

        public Setting()
        {
            _getStringDelegates = new Dictionary<string, GetStringDelegate>();
            _setStringDelegates = new Dictionary<string, SetStringDelegate>();

            var properties = GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                _getStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(GetStringDelegate), this, property.GetGetMethod()) as GetStringDelegate);
                _setStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(SetStringDelegate), this, property.GetSetMethod()) as SetStringDelegate);
            }
        }

        public string this[string propertyName]
        {
            get { return _getStringDelegates[propertyName](); }
            set { _setStringDelegates[propertyName](value); }
        }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
