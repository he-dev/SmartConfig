using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reusable.Extensions;

namespace SmartConfig.Data
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Setting
    {
        // We store all names and values in a non-case sensitive dictionary.
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public string WeakId => $"{Name.WeakFullName}/{Attributes.OrderBy(x => x.Key).Select(x => x.Value).Join("/")}";

        public string StrongId => $"{Name.StrongFullName}/{Attributes.OrderBy(x => x.Key).Select(x => x.Value).Join("/")}";

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

        private string DebuggerDisplay => ToString();       

        public override string ToString()
        {
            return $"{Name.StrongFullName} = '{Value}' in [{string.Join(",", Attributes.Select(x => x.Value))}]";
        }        
    }
}
