using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Indicates that a it is optional to provide a value for this field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionalAttribute : ValueContraintAttribute
    {
    }
}
