using System;

namespace SmartConfig
{
    /// <summary>
    /// Fields or classes with this attribute are not loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
