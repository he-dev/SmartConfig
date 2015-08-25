using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint exceptions.
    /// </summary>
    public abstract class ConstraintException : Exception
    {
        public ConstraintException(ConstraintAttribute constraint, object value)
        {
            Constraint = constraint;
            Value = value;
        }

        public override string Message
        {
            get
            {
                return "Constraint [$AbbreviatedAttributeName] failed.".FormatWith(new { AbbreviatedAttributeName }, true);
            }
        }

        public ConstraintAttribute Constraint { get; private set; }

        /// <summary>
        /// Gets the invalid value that caused the exception.
        /// </summary>
        public object Value { get; private set; }

        private string AbbreviatedAttributeName
        {
            get { return Regex.Replace(Constraint.GetType().Name, "Attribute$", string.Empty); }
        }
    }
}
