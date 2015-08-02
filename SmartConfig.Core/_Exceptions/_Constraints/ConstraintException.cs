using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint exceptions.
    /// </summary>
    public abstract class ConstraintException : Exception
    {
        public ConstraintException(object value, string message)
            : base(message)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the invalid value that caused the exception.
        /// </summary>
        public object Value { get; protected set; }
    }
}
