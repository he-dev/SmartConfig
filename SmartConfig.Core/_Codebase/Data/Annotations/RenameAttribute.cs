using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class RenameAttribute : Attribute
    {
        public RenameAttribute(string name)
        {
            Name = name.Validate(nameof(name)).IsNotNullOrEmpty().Value;
        }

        public string Name { get; }
    }
}
