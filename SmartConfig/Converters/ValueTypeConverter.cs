﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class ValueTypeConverter : ObjectConverterBase
    {
        //public ValueTypeConverter()
        //{
        //    //new[] 
        //    //{ 
        //    //    typeof(Char),
        //    //    typeof(Char?),

        //    //    typeof(Byte),
        //    //    typeof(Byte?),
                
        //    //    typeof(SByte),
        //    //    typeof(SByte?),

        //    //    typeof(Int16),
        //    //    typeof(Int16?),
        //    //    typeof(Int32),
        //    //    typeof(Int32?),
        //    //    typeof(Int64),
        //    //    typeof(Int64?),
                
        //    //    typeof(UInt16),
        //    //    typeof(UInt16?),
        //    //    typeof(UInt32),
        //    //    typeof(UInt32?),
        //    //    typeof(UInt64),
        //    //    typeof(UInt64?),

        //    //    typeof(Boolean),
        //    //    typeof(Boolean?),
        //    //}
        //    //.Select(t => SupportedTypes.Add(t))
        //    //.ToList();
        //}

        protected override bool CanConvert(Type type)
        {
            return type.IsValueType || type.IsNullable();
        }

        public override object DeserializeObject(string value, Type type)
        {
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            if (type.IsNullable())
            {
                if (string.IsNullOrEmpty(value))
                {
                    // It is ok to return null for nullable types.
                    return null;
                }
                type = Nullable.GetUnderlyingType(type);
            }

            var parseMethod = type.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
            if (parseMethod != null)
            {
                var result = parseMethod.Invoke(null, new object[] { value, CultureInfo.InvariantCulture });
                return result;
            }
            else
            {
                parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) });
                if (parseMethod != null)
                {
                    var result = parseMethod.Invoke(null, new object[] { value });
                    return result;
                }
            }
            throw new Exception("Parse method not found.");
        }

        public override string SerializeObject(object value)
        {
            if (value == null)
            {
                // It is ok to return null for null objects.
                return null;
            }

            var type = value.GetType();
            if (!CanConvert(type))
            {
                throw new InvalidOperationException(string.Format("Type [{0}] is not supported.", type.Name));
            }

            var toStringMethod = type.GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
            if (toStringMethod != null)
            {
                var result = toStringMethod.Invoke(value, new object[] { CultureInfo.InvariantCulture });
                return (string)result;
            }
            else
            {
                toStringMethod = type.GetMethod("ToString", new Type[] { });
                var result = toStringMethod.Invoke(value, null);
                return (string)result;
            }

            throw new Exception("ToString method not found.");
        }
    }
}