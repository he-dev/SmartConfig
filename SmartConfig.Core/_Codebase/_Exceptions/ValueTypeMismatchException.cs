using System;
using System.Runtime.Serialization;
using SmartUtilities;

namespace SmartConfig
{
    [Serializable]
    public class ValueTypeMismatchException : SmartException
    {
        public string ValueTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string TargetTypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }
}