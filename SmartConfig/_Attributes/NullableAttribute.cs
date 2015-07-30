using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Indicates that the value of the field can be null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class NullableAttribute : Attribute
    {
    }
}
