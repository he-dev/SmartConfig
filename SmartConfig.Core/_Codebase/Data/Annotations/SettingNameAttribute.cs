using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    // Allows to specify an alternative setting name.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SettingNameAttribute : Attribute
    {
        private readonly string _name;

        public SettingNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            _name = name;
        }

        public override string ToString() => _name;

        public static implicit operator string(SettingNameAttribute attribute) => attribute.ToString();
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class SettingNameUnsetAttribute : SettingNameAttribute
    {
        public SettingNameUnsetAttribute():base(SettingName.Unset)
        {
        }
    }
}
