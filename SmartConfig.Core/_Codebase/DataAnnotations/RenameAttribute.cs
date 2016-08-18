using System;
using SmartUtilities.Frameworks.InlineValidation;

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
