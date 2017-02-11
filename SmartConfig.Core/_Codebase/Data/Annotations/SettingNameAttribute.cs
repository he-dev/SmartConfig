using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    // Allows to specify an alternative setting name.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SettingNameAttribute : Attribute
    {
        public SettingNameAttribute(string name)
        {
            Name = name.Validate(nameof(name)).IsNotNullOrEmpty().Value;
        }

        public string Name { get; }

        public override string ToString() => Name;

        public static implicit operator string(SettingNameAttribute attribute) => attribute.ToString();
    }
}
