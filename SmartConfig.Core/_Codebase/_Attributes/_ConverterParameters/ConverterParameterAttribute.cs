using System;

namespace SmartConfig
{
    /// <summary>
    /// Base class for converter parameter attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ConverterParameterAttribute : Attribute
    {
    }
}
