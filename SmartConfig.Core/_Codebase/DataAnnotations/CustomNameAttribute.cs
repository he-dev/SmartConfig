using System;

namespace SmartConfig.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class CustomNameAttribute : Attribute
    {
        public CustomNameAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

            Name = name;
        }

        public string Name { get; }
    }
}
