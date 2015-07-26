using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Converters
{
    public class EnumConverter : ObjectConverterBase
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

        public EnumConverter()
        {
            FieldTypes = new HashSet<Type>()
            {
                typeof(Enum),
            };
        }

        protected override bool CanConvert(Type type)
        {
            return type.BaseType == typeof(Enum) || type.IsNullable();
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
                // Otherwise get the underlying type.
                type = Nullable.GetUnderlyingType(type);
            }

            var result = Enum.Parse(type, value);
            return result;
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

            var result = value.ToString();
            return result;
        }
    }
}
