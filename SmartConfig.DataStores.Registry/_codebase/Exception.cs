using System;
using System.Collections.Generic;
using System.Linq;
using SmartUtilities;

namespace SmartConfig.DataStores.Registry
{
    public class UnsupportedTypeException : Exception
    {
        internal UnsupportedTypeException(IEnumerable<string> settingNames, IEnumerable<Type> allowedTypes)
        {
            Message = 
                $"There are some setting with unsupported types: [{string.Join(", ", settingNames)}]. " +
                $"Allowed types are: [{string.Join(", ", allowedTypes.Select(x => x.Name))}].";
        }

        public override string Message { get; }

        public override string ToString() => this.ToJson();
    }    
}
