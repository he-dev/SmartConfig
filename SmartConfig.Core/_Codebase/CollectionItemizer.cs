using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartUtilities;
using SmartUtilities.Frameworks.InfiniteConversion;

namespace SmartConfig
{
    internal class CollectionItemizer
    {
        private static readonly Func<object, Func<Type, Type>, TypeConverter, IEnumerable<KeyValuePair<string, object>>>[] Factories =
        {
            ItemizeArray,
            ItemizeList,
            ItemizeHashSet,
            ItemizeDictionary,
        };

        public static IEnumerable<KeyValuePair<string, object>> ItemizeCollection(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            return
                Factories
                    .Select(factory => factory(settings, mapDataType, converter))
                    .FirstOrDefault(collection => collection != null);
        }

        public static IEnumerable<KeyValuePair<string, object>> ItemizeArray(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsArray) { return null; }

            var elementType = settings.GetType().GetElementType();
            var dataType = mapDataType(elementType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select((x, i) => new KeyValuePair<string, object>(
                    key: i.ToString(), 
                    value: converter.Convert(x, dataType)
                ));

            return result;
        }

        public static IEnumerable<KeyValuePair<string, object>> ItemizeList(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsList()) { return null; }

            var elementType = settings.GetType().GetGenericArguments()[0];
            var dataType = mapDataType(elementType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select((x, i) => new KeyValuePair<string, object>(
                    key: i.ToString(), 
                    value: converter.Convert(x, dataType)
                ));

            return result;
        }

        public static IEnumerable<KeyValuePair<string, object>> ItemizeHashSet(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsHashSet()) { return null; }

            var elementType = settings.GetType().GetGenericArguments()[0];
            var dataType = mapDataType(elementType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select((x, i) => new KeyValuePair<string, object>(
                    key: i.ToString(), 
                    value: converter.Convert(x, dataType)
                ));

            return result;
        }

        public static IEnumerable<KeyValuePair<string, object>> ItemizeDictionary(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsDictionary()) { return null; }

            var valueType = settings.GetType().GetGenericArguments()[1];
            var dataType = mapDataType(valueType);

            var dictionary = (IDictionary)settings;

            var result = dictionary.Keys.Cast<object>().Select(
                key => new KeyValuePair<string, object>(
                    key: (string)converter.Convert(key, typeof(string)),
                    value: converter.Convert(dictionary[key], dataType)
            ));

            return result;
        }
    }

    //internal class CollectionItemizer2
    //{
    //    private static readonly Func<object, object>[] Factories =
    //    {
    //        ItemizeArray,
    //        ItemizeList,
    //        ItemizeHashSet,
    //        ItemizeDictionary,
    //    };

    //    public static object ItemizeCollection(object settings)
    //    {
    //        return
    //            Factories
    //                .Select(factory => factory(settings))
    //                .FirstOrDefault(collection => collection != null);
    //    }

    //    public static object ItemizeArray(object settings)
    //    {
    //        if (!settings.GetType().IsArray) { return null; }
    //        var result = ((IEnumerable)settings).Cast<object>().Select(x => x);
    //        return result;
    //    }

    //    public static object ItemizeList(object settings)
    //    {
    //        if (!settings.GetType().IsList()) { return null; }
    //        var result = ((IEnumerable)settings).Cast<object>().Select(x => x);
    //        return result;
    //    }

    //    public static object ItemizeHashSet(object settings)
    //    {
    //        if (!settings.GetType().IsHashSet()) { return null; }
    //        var result = ((IEnumerable)settings).Cast<object>().Select(x => x);
    //        return result;
    //    }

    //    public static object ItemizeDictionary(object settings)
    //    {
    //        if (!settings.GetType().IsDictionary()) { return null; }
    //        var dictionary = (IDictionary)settings;
    //        var result = dictionary.Keys.Cast<object>().ToDictionary(key => key, key => dictionary[key]);
    //        return result;
    //    }
    //}
}
