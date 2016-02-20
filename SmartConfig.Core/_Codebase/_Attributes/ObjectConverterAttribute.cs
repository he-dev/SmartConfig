using System;
using SmartConfig.Converters;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the type of a custom object converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ObjectConverterAttribute : Attribute
    {
        public ObjectConverterAttribute(Type converterType)
        {
            if (converterType == null) { throw new ArgumentNullException(nameof(converterType)); }

            if (!typeof(ObjectConverter).IsAssignableFrom(converterType))
            {
                throw new TypeDoesNotImplementInterfaceException
                {
                    ExpectedType = typeof(ObjectConverter).FullName,
                    ActualType = converterType.FullName
                };
            }

            Type = converterType;
        }

        /// <summary>
        /// Gets the object converter type.
        /// </summary>
        public Type Type { get; }
    }
}
