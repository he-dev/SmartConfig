using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartConfig
{
    /// <summary>
    /// Specifies the type of a custom object converter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ObjectConverterAttribute : Attribute
    {
        public ObjectConverterAttribute(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the object converter type.
        /// </summary>
        public Type Type { get; private set; }
    }
}
