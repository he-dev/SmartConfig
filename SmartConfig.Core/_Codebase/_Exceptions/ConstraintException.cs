using System;
using System.Text.RegularExpressions;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint exceptions.
    /// </summary>
    public class ConstraintException : Exception
    {
        public ConstraintException(ConstraintAttribute constraint, object value)
        {
            Constraint = constraint;
            Value = value;
        }

        public override string Message => "Constraint \"$AbbreviatedAttributeName\" failed for $Constraint"
            .FormatWith(new
            {
                AbbreviatedAttributeName,
                Constraint = Constraint.ToString()
            }, true);

        public ConstraintAttribute Constraint { get; private set; }

        /// <summary>
        /// Gets the invalid value that caused the exception.
        /// </summary>
        public object Value { get; private set; }

        private string AbbreviatedAttributeName => Regex.Replace(Constraint.GetType().Name, "Attribute$", string.Empty);
    }
}
