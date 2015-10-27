using System;

namespace SmartConfig
{
    /// <summary>
    /// Indicates that a field is optional. You should provide a default value in this case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionalAttribute : Attribute
    {
    }
}
