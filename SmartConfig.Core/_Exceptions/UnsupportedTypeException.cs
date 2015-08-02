using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a type was used that is not supported by an object converter.
    /// </summary>
    public class UnsupportedTypeException : Exception
    {
        public UnsupportedTypeException(Type type)
            : base(string.Format("Type [{0}] is not supported.", type.Name))
        {
            Type = type;
        }

        /// <summary>
        /// Gets the unsupported type.
        /// </summary>
        public Type Type { get; private set; }
    }
}
