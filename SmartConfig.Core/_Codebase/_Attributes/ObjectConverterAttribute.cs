using System;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the type of a custom object converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ObjectConverterAttribute : Attribute
    {
        public ObjectConverterAttribute(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            Type = type;
        }

        /// <summary>
        /// Gets the object converter type.
        /// </summary>
        public Type Type { get; }
    }
}
