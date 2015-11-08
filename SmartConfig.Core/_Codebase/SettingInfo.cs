﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SmartConfig.Reflection;
using SmartUtilities;

namespace SmartConfig
{
    [DebuggerDisplay("ConfigType = {Configuration.ConfigType.Name} SettingPath = \"{SettingPath}\" IsInternal = \"{IsInternal}\"")]
    public class SettingInfo
    {
        internal SettingInfo(ConfigurationInfo configuration, PropertyInfo property, IEnumerable<string> path)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(property != null);

            Configuration = configuration;
            Property = property;

            SettingPath = new SettingPath(path);
        }      

        internal ConfigurationInfo Configuration { get; }

        internal PropertyInfo Property { get; }

        public Type SettingType => Property.PropertyType;

        public Type ConverterType
        {
            get
            {
                if (IsInternal)
                {
                    return SettingType;
                }

                if (SettingType.BaseType == typeof(Enum))
                {
                    return typeof(Enum);
                }

                var objectConverterAttribute = Property.GetCustomAttribute<ObjectConverterAttribute>(false);
                if (objectConverterAttribute != null)
                {
                    return objectConverterAttribute.Type;
                }

                return SettingType;
            }
        }

        public SettingPath SettingPath { get; }

        public IEnumerable<ConstraintAttribute> SettingConstraints =>
            Property == null
            ? Enumerable.Empty<ConstraintAttribute>()
            : Property.GetCustomAttributes<ConstraintAttribute>(false);

        public bool IsOptional => Property.GetCustomAttribute<OptionalAttribute>() != null;

        //public bool IsNullable
        //{
        //    get
        //    {
        //        var isNullable =
        //            (_property.FieldType.IsValueType && _property.FieldType.IsNullable())
        //            || _property.GetCustomAttribute<OptionalAttribute>() != null;
        //        return isNullable;
        //    }
        //}

        public object Value
        {
            get { return Property.GetValue(null); }
            set { Property.SetValue(null, value); }
        }

        internal bool IsInternal => Property == null;

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
