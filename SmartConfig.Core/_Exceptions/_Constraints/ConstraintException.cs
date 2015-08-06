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
    public abstract class ConstraintException<TConstraint> : Exception where TConstraint : ConstraintAttribute
    {
        public ConstraintException(TConstraint constraint, object value)
        {
            Value = value;
        }

        public override string Message
        {
            get
            {
                return "Constraint '$AbbreviatedAttributeName' failed for value '$Value'.".FormatWith(new { AbbreviatedAttributeName, Value }, true);
            }
        }

        public TConstraint Constraint { get; private set; }

        /// <summary>
        /// Gets the invalid value that caused the exception.
        /// </summary>
        public object Value { get; private set; }

        private string AbbreviatedAttributeName
        {
            get { return Regex.Replace(typeof(TConstraint).Name, "Attribute$", string.Empty); }
        }
    }
}
