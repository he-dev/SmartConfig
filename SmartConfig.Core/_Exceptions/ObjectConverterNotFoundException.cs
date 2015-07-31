using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartConfig
{
    public class ObjectConverterNotFoundException : Exception
    {
        public ObjectConverterNotFoundException(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type for which a converter could not be found.
        /// </summary>
        public Type Type { get; private set; }
    }
}
