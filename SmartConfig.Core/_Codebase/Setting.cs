using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Represents a single setting info.
    /// </summary>
    [DebuggerDisplay("Configuration = {Configuration.Type.Name} SettingPath = \"{Path}\"")]
    internal class Setting
    {
        public Setting(PropertyInfo propertyInfo, Configuration configuration)
        {
            Property = propertyInfo;
            Configuration = configuration;

            var path = propertyInfo.GetSettingPath();
            Path = new SettingPath(Configuration.Name, path);
        }

        internal Configuration Configuration { get; }

        internal PropertyInfo Property { get; }

        public Type Type => Property.PropertyType;

        public Type ConverterType
        {
            get
            {
                if (Type.BaseType == typeof(Enum))
                {
                    return typeof(Enum);
                }

                var objectConverterAttribute = Property.GetCustomAttribute<ObjectConverterAttribute>(false);
                return objectConverterAttribute != null ? objectConverterAttribute.Type : Type;
            }
        }

        public SettingPath Path { get; }

        public CompoundSettingKey Key => new CompoundSettingKey(
            new SimpleSettingKey(BasicSetting.DefaultKeyName, Path),
            Configuration.CustomKeys);

        public IEnumerable<Attribute> Atributes => Property?.GetCustomAttributes(false).Cast<Attribute>() ?? Enumerable.Empty<Attribute>();

        public bool IsOptional => Property.GetCustomAttribute<OptionalAttribute>() != null;

        public bool Loaded => !IsOptional && Value != null;
       
        public object Value
        {
            get { return Property.GetValue(null); }
            set { Property.SetValue(null, value); }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Setting;
            return other != null && other.Property == Property;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode();
        }
    }
}
