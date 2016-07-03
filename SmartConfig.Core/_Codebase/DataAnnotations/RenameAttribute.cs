using System;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class RenameAttribute : Attribute
    {
        public RenameAttribute(string name)
        {
            Name = name.Validate(nameof(name)).IsNotNullOrEmpty().Argument;
        }

        public string Name { get; }
    }
}
