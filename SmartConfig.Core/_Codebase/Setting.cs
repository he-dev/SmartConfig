using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig
{
    /// <summary>
    /// Represents a single setting info.
    /// </summary>
    [DebuggerDisplay("Configuration = {Configuration.Type.Name} SettingPath = \"{Path}\"")]
    internal class Setting
    {
        private SettingPath _settingPath;

        internal Configuration Configuration { get; private set; }

        internal PropertyInfo Property { get; private set; }

        public Type Type => Property.PropertyType;

        // in some cases we need the base type instead of the actual type
        public Type NormalizedType => Type.BaseType == typeof(Enum) ? Type.BaseType : Type;

        public SettingPath Path => _settingPath ?? (_settingPath = SettingPath.Create(Configuration.Name, Property.GetPropertyPath()));

        public SettingKey Key => new SettingKey(Path, Configuration.DataStore.CustomKeyValues);

        public IEnumerable<Attribute> Atributes => Property?.GetCustomAttributes(false).Cast<Attribute>() ?? Enumerable.Empty<Attribute>();

        public bool IsOptional => Property.GetCustomAttribute<OptionalAttribute>() != null;

        public object Value
        {
            get { return Property.GetValue(null); }
            set { Property.SetValue(null, value); }
        }

        public static Setting Create(PropertyInfo property, Configuration configuration)
        {
            return new Setting
            {
                Property = property,
                Configuration = configuration,
            };
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
