using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Base class for constraint attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ConstraintAttribute : Attribute
    {

    }
}
