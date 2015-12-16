using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig.Converters
{
    /// <summary>
    /// Base class for converters. Provides a helper method for type verification.
    /// </summary>
    public abstract class ObjectConverter
    {
        protected ObjectConverter()
        {
            // by default the only supported type is the own type 
            // currently used only by the Json converter that supports "all" types natively
            SupportedTypes.Add(GetType());
        }

        protected ObjectConverter(IEnumerable<Type> supportedTypes)
        {
            SupportedTypes.Clear();
            SupportedTypes.UnionWith(supportedTypes);
        }

        /// <summary>
        /// Gets field types directly supported by the converter. 
        /// If left empty the conveter type will by used and you need to set the <c>ObjectConverterAttributes</c> on your field(s).
        /// </summary>
        public HashSet<Type> SupportedTypes { get; } = new HashSet<Type>();

        //protected void ValidateType(Type type)
        //{
        //    if (type.IsEnum)
        //    {
        //        type = typeof(Enum);
        //    }

        //    if (!SupportedTypes.Contains(type))
        //    {
        //        throw new UnsupportedTypeException(GetType(), type);
        //    }
        //}

        /// <summary>
        /// Deserializes the value to the specified type.
        /// </summary>
        /// <param name="value">Value to be deserialized.</param>
        /// <param name="type">SettingType of the value.</param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <remarks>It is not necessary to check for null value. <c>configuration</c> dosn't pass null values.</remarks>
        public abstract object DeserializeObject(object value, Type type, IEnumerable<Attribute> attributes);

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <param name="value">Value to be serialized.</param>
        /// <param name="type"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        /// <remarks>It is not necessary to check for null value. <c>configuration</c> dosn't pass null values.</remarks>
        public abstract object SerializeObject(object value, Type type, IEnumerable<Attribute> attributes);

        protected void Validate(object value, IEnumerable<Attribute> attributes)
        {
            foreach (var constraint in attributes.OfType<ConstraintAttribute>())
            {
                constraint.Validate(value);
            }
        }

    }
}
