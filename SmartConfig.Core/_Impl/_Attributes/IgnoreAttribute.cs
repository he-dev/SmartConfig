using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Fields or classes with this attribute are not loaded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class IgnoreAttribute : Attribute
    {
    }
}
