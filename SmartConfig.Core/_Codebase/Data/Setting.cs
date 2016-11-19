using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmartConfig.Data
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Setting
    {
        // We store all names and values in a not-case sensitive dictionary.
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public SettingUrn Name
        {
            [DebuggerStepThrough]
            get { return (SettingUrn)_values[nameof(Name)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Name)] = value; }
        }

        public object Value
        {
            [DebuggerStepThrough]
            get { return _values[nameof(Value)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Value)] = value; }
        }

        public IReadOnlyDictionary<string, object> Attributes
        {
            get
            {
                var defaultPropertyNames = new[] { nameof(Name), nameof(Value) };
                var namespaces = _values.Where(x => !defaultPropertyNames.Contains(x.Key));
                return namespaces.ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            }
            set
            {
                var attributes = Attributes.ToDictionary(x => x.Key, x => x.Value);
                foreach (var item in attributes)
                {
                    _values.Remove(item.Key);
                }
                foreach (var item in value)
                {
                    _values.Add(item.Key, item.Value);
                }
            }
        }

        public IEnumerable<string> Names => _values.Keys;

        private string DebuggerDisplay => ToString();

        public bool NamespaceEquals(string key, object value)
        {
            var temp = (object)null;
            return Attributes.TryGetValue(key, out temp) && temp.Equals(value);
        }

        // Two settings are alike if their Name.FullNames are same and all namespaces and their values.
        //public bool IsLike(Setting setting)
        //{
        //    return
        //        Name.IsLike(setting.Name) &&
        //        Attributes.All(ns => setting.NamespaceEquals(ns.Key, ns.Value));
        //}

        public override string ToString()
        {
            return $"{Name.FullNameWithKey} = '{Value}' in [{string.Join(",", Attributes.Select(x => x.Value))}]";
        }

        //public static bool operator ==(Setting x, Setting y)
        //{
        //    if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;

        //    return 
        //        x.na;
        //}

        //public static bool operator !=(Setting x, Setting y)
        //{
        //    return !(x == y);
        //}
    }
}
