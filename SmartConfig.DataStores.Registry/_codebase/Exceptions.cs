﻿using System;
using System.Collections.Generic;
using System.Linq;
using Reusable;

namespace SmartConfig.DataStores.Registry
{
    public class InvalidTypeException : Exception
    {
        internal InvalidTypeException(Type invalidType, IEnumerable<Type> validTypes)
        {
            InvalidType = invalidType;
            ValidTypes = validTypes;
        }

        public Type InvalidType { get; }

        public IEnumerable<Type> ValidTypes { get; }

        public override string Message => $"Invalid type \"{InvalidType.Name}\". Valid types are [{string.Join(", ", ValidTypes.Select(x => x.Name))}].";

        //public override string ToString() => this.ToJson();
    }

    public class OpenOrCreateSubKeyException : Exception
    {
        internal OpenOrCreateSubKeyException(string baseKeyName, string baseKeySubName, string subKeyName)
        {
            BaseKeyName = baseKeyName;
            BaseKeySubName = baseKeySubName;
            SubKeyName = subKeyName;
        }

        public string BaseKeyName { get; }
        public string BaseKeySubName { get; set; }
        public string SubKeyName { get; }

        public override string Message => $"Could not open or create \"{BaseKeyName}\\{BaseKeySubName}\\{SubKeyName}\".";
    }
}
