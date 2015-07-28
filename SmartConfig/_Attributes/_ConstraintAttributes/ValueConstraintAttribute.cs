using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class ValueConstraintAttribute : Attribute
    {

    }
}
