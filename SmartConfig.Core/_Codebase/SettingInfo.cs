using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;
using SmartUtilities.DataAnnotations;
using SmartUtilities.TypeFramework;

namespace SmartConfig
{
    // stores information about a single setting
    [DebuggerDisplay("Path = \"{SettingPath}\"")]
    internal class SettingInfo
    {
        internal SettingInfo(PropertyInfo property)
        {
            Property = property;
            SettingPath = new SettingPath(Property.GetSettingPath().ToList());
            ValidationAttributes = Property.GetCustomAttributes<ValidationAttribute>();
            IsOptional = Property.GetCustomAttribute<OptionalAttribute>() != null;
        }

        public PropertyInfo Property { get; }

        public Type Type => Property.PropertyType;

        public SettingPath SettingPath { get; }

        public IEnumerable<ValidationAttribute> ValidationAttributes { get; }

        public bool IsOptional { get; }

        public object Value
        {
            get { return Property.GetValue(null); }
            set { Property.SetValue(null, value); }
        }

        public override bool Equals(object obj)
        {
            var other = obj as SettingInfo;
            return other != null && other.Property == Property;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }
    }
}
