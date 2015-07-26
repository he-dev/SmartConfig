using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Specifies that a reference type field can be null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AllowNullAttribute : ValueContraintAttribute
    {
    }
}
