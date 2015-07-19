using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartConfig
{
    public class ConverterNotFoundException : Exception
    {
        public ConverterNotFoundException(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type of the converter that could not be found.
        /// </summary>
        public Type Type { get; private set; }
    }
}
