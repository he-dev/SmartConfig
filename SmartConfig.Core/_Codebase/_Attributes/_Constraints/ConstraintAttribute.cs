using System;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ConstraintAttribute : Attribute
    {
        public abstract string Properties { get; }
    }
}
