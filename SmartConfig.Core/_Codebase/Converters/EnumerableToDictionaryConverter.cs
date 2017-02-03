using System;
using System.Collections;
using System.Collections.Generic;
using Reusable;
using Reusable.Converters;

namespace SmartConfig.Converters
{
    internal class EnumerableToDictionaryConverter : TypeConverter<IEnumerable, IDictionary>
    {
        public override bool CanConvert(object value, Type targetType)
        {
            return value.GetType().IsEnumerable();
        }

        protected override IDictionary ConvertCore(IConversionContext<IEnumerable> context)
        {
            if (context.Value.GetType().IsDictionary())
            {
                return (IDictionary)context.Value;
            }
            else
            {
                var result = CreateDictionary();
                var index = -1;
                foreach (var element in context.Value)
                {
                    result.Add(++index, element);
                }

                return result;
            }
        }       

        private static IDictionary CreateDictionary()
        {
            var keyType = typeof(object);
            var valueType = typeof(object);

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var result = (IDictionary)Activator.CreateInstance(dictionaryType);
            return result;
        }
    }
}