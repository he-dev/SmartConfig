using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;
using SmartUtilities;
using SmartUtilities.DataAnnotations;
using SmartUtilities.TypeFramework;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig
{
    // stores information about a single setting
    [DebuggerDisplay("Path = \"{SettingPath}\"")]
    internal class SettingInfo
    {
        internal SettingInfo(PropertyInfo property, Type configType)
        {
            Property = property;
            ConfigType = configType;
            SettingPath = new SettingPath(Property.GetSettingPath().ToList());
            //ValidationAttributes = Property.GetCustomAttributes<ValidationAttribute>();

            // an itemzed setting must be an enumerable
            if (IsItemized)
            {
                IsEnumerable.Validate(nameof(IsEnumerable)).IsTrue();
            }
        }

        public PropertyInfo Property { get; }

        public Type ConfigType { get; }

        public bool IsEnumerable => Property.PropertyType.IsEnumerable();

        public bool IsItemized => Property.GetCustomAttribute<ItemizedAttribute>() != null;

        public Type Type => Property.PropertyType;

        public SettingPath SettingPath { get; }

        //public IEnumerable<ValidationAttribute> ValidationAttributes { get; }

        public bool IsOptional => Property.GetCustomAttribute<OptionalAttribute>() != null;

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

        public override string ToString()
        {
            // FullConfig2.Foo.Bar[baz] (required) System.Collections.Generic.List < int >
            return $"{ConfigType.Name}.{SettingPath.FullNameEx} {(IsOptional ? "optional" : "required")} {Type.ToShortString()}";
        }
    }
}
