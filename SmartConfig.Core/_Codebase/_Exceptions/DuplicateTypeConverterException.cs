using System;
using System.Runtime.Serialization;
using SmartUtilities;

namespace SmartConfig
{
    [Serializable]
    public class DuplicateTypeConverterException : SmartException
    {
        public string ConverterFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string TypeName { get { return GetValue<string>(); } internal set { SetValue(value); } }

    }
}