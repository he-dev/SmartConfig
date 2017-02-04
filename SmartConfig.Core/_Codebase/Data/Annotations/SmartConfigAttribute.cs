using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    // Decorates the main config class. This attribute is required.
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public SmartConfigAttribute(SettingNameTarget settingNameTarget)
        {
            SettingNameTarget = settingNameTarget;
        }

        public SmartConfigAttribute() : this(SettingNameTarget.None) { }

        public SettingNameTarget SettingNameTarget { get; }
    }
}
