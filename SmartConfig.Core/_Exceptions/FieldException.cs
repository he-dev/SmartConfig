using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class FieldException : Exception
    {
        public FieldException(Type configType, FieldInfo fieldInfo)
        {

        }

        public Type ConfigType { get; private set; }

        internal FieldInfo FieldInfo { get; set; }
    }
}
