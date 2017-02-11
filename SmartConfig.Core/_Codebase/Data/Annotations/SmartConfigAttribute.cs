using System;
using Reusable;
using Reusable.Fuse;

namespace SmartConfig.Data.Annotations
{
    // Decorates the main config class. This attribute is required.
    [AttributeUsage(AttributeTargets.Class)]
    public class SmartConfigAttribute : Attribute
    {
        
    }
}
