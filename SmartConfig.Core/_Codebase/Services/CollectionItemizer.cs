using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reusable;
using Reusable.Converters;

namespace SmartConfig.Services
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

    //internal class CollectionItemizer
    //{
    //    private static readonly Func<object, IEnumerable<KeyValuePair<object, object>>>[] Itemizers =
    //    {
    //        ItemizeArray,
    //        ItemizeList,
    //        ItemizeHashSet,
    //        ItemizeDictionary,
    //    };

    //    public static IEnumerable<KeyValuePair<object, object>> ItemizeArray(object obj)
    //    {
    //        if (!obj.GetType().IsArray) { return null; }

    //        var result =
    //            ((IEnumerable)obj)
    //            .Cast<object>()
    //            .Select((value, key) => new KeyValuePair<object, object>(key, value));

    //        return result;
    //    }

    //    public static IEnumerable<KeyValuePair<object, object>> ItemizeList(object obj)
    //    {
    //        if (!obj.GetType().IsList()) { return null; }

    //        var result =
    //            ((IEnumerable)obj)
    //            .Cast<object>()
    //            .Select((value, key) => new KeyValuePair<object, object>(key, value));

    //        return result;
    //    }

    //    public static IEnumerable<KeyValuePair<object, object>> ItemizeHashSet(object obj)
    //    {
    //        if (!obj.GetType().IsHashSet()) { return null; }

    //        var result =
    //            ((IEnumerable)obj)
    //            .Cast<object>()
    //            .Select((value, key) => new KeyValuePair<object, object>(key, value));

    //        return result;
    //    }

    //    public static IEnumerable<KeyValuePair<object, object>> ItemizeDictionary(object obj)
    //    {
    //        if (!obj.GetType().IsDictionary()) { return null; }

    //        var dictionary = (IDictionary)obj;

    //        var result = dictionary.Keys
    //            .Cast<object>()
    //            .Select(key => new KeyValuePair<object, object>(key: key, value: dictionary[key]));

    //        return result;
    //    }
    //}
}
