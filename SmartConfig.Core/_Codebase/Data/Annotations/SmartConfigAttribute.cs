using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public SmartConfigAttribute() { }

        public SmartConfigAttribute(string name, ConfigurationNameTarget nameTarget)
        {
            Name = name.Validate(nameof(name)).IsNotNullOrEmpty().Value;
            NameTarget = nameTarget;
        }

        public SmartConfigAttribute(string name) : this(name, ConfigurationNameTarget.Path) { }

        public string Name { get; }

        public ConfigurationNameTarget NameTarget { get; }
    }
}
