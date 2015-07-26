using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(Type type)
            : base(string.Format("Type [{0}] is not supported.", type.Name))
        {
            Type = type;
        }

        public Type Type { get; private set; }
    }
}
