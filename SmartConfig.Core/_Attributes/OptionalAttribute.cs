using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Indicates that a field is optional. You should provide a default value in this case.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class OptionalAttribute : Attribute
    {
    }
}
