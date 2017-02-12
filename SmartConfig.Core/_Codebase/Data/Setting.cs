using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Reusable.Fuse;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    // Data model for a setting in a data store.
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Setting
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public SettingPath Name
        {
            [DebuggerStepThrough]
            get { return (SettingPath)_values[nameof(Name)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Name)] = value.Validate(nameof(Name)).IsNotNull().Value; }
        }

        public object Value
        {
            [DebuggerStepThrough]
            get { return _values[nameof(Value)]; }

            [DebuggerStepThrough]
            set { _values[nameof(Value)] = value; }
        }

        public TagCollection Tags { get; set; } = new TagCollection();

        private string DebuggerDisplay => ToString();

        public override string ToString()
        {
            return $"{Name.StrongFullName} = '{Value}' in [{string.Join(",", Tags.Select(x => x.Value))}]";
        }
    }
}
