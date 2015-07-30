using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    /// <summary>
    /// Is thrown if there is no type marked with the <c>SmartConfigAttribute</c>.
    /// </summary>
    public class SmartConfigTypeNotFoundException : Exception
    {
        public SmartConfigTypeNotFoundException(string message) : base(message) { }
    }
}
