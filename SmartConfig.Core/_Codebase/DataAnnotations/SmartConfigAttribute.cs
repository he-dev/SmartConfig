using System;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataAnnotations
{
    /// <summary>
    /// Marks a type as <c>SmartConfig</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        public SmartConfigAttribute() { }

        public SmartConfigAttribute(string name)
        {
            Name = name.Validate(nameof(name)).IsNotNullOrEmpty().Argument;
        }

        public string Name { get; internal set; }

        public ConfigNameOption NameOption { get; set; } = ConfigNameOption.AsPath;
    }

    public enum ConfigNameOption
    {
        AsPath,
        AsNamespace
    }
}
