using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class NullableException : ConstraintException
    {
        public NullableException(object value)
            : base(value, "Value is not nullable. If you want it to be nuallable add the NullableAttribute.")
        {
        }
    }
}
