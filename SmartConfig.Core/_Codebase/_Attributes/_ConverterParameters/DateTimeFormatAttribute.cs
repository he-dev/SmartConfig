using System;
using System.Diagnostics;
using System.Globalization;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Allows to define an exact date time format.
    /// </summary>
    [DebuggerDisplay("Format = \"{Format}\"")]
    public class DateTimeFormatAttribute : ConverterParameterAttribute
    {
        /// <summary>
        /// Gets the format set in the constructor.
        /// </summary>
        public string Format { get; set; }        
    }
}
