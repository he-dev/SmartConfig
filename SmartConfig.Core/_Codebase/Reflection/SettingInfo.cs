﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartConfig.Paths;

namespace SmartConfig.Reflection
{
    /// <summary>
    /// Represents a single setting info.
    /// </summary>
    [DebuggerDisplay("ConfigurationType = {Configuration.ConfigurationType.Name} SettingPath = \"{SettingPath}\"")]
    internal class SettingInfo
    {
        public SettingInfo(PropertyInfo propertyInfo, ConfigurationInfo configurationInfo)
        {
            Property = propertyInfo;
            Configuration = configurationInfo;
        }

        internal ConfigurationInfo Configuration { get; }

        internal PropertyInfo Property { get; }

        public Type SettingType => Property.PropertyType;

        public Type ConverterType
        {
            get
            {
                if (SettingType.BaseType == typeof(Enum))
                {
                    return typeof(Enum);
                }

                var objectConverterAttribute = Property.GetCustomAttribute<ObjectConverterAttribute>(false);
                return objectConverterAttribute != null ? objectConverterAttribute.Type : SettingType;
            }
        }

        public SettingPath SettingPath
        {
            get
            {
                var path = Property.GetSettingPath();
                return new SettingPath(Configuration.ConfigurationName, path);
            }
        }

        public SettingKeyCollection Keys => new SettingKeyCollection(
            new SettingKey(Setting.DefaultKeyName, SettingPath),
            Configuration.ConfigurationPropertyGroup.CustomKeys);

        public IEnumerable<Attribute> SettingCustomAttributes => Property?.GetCustomAttributes(false).Cast<Attribute>() ?? Enumerable.Empty<Attribute>();

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
    }
}
