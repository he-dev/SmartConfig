using System;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ConstraintAttribute : Attribute
    {
        public abstract void Validate(object value);
    }
}
