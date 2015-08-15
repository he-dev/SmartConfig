using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace SmartConfig.Data
{
    public delegate string GetStringDelegate();
    public delegate void SetStringDelegate(string value);

    /// <summary>
    /// This is the default basic config element. Custom config elements must be derived from this type.
    /// </summary>
    public class ConfigElement
    {

        public ConfigElement() { }

        protected ConfigElement(Type type)
        {
            GetStringDelegates = new Dictionary<string, GetStringDelegate>();
            SetStringDelegates = new Dictionary<string, SetStringDelegate>();
            CustomProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in CustomProperties)
            {
                GetStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(GetStringDelegate), this, property.GetGetMethod()) as GetStringDelegate);
                SetStringDelegates.Add(property.Name, Delegate.CreateDelegate(typeof(SetStringDelegate), this, property.GetSetMethod()) as SetStringDelegate);
            }
        }

        public PropertyInfo[] CustomProperties { get; private set; }
        public IDictionary<string, GetStringDelegate> GetStringDelegates { get; private set; }
        public IDictionary<string, SetStringDelegate> SetStringDelegates { get; private set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
