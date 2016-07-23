﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartConfig.Data;
using SmartUtilities.TypeFramework;

namespace SmartConfig
{
    internal class CollectionItemizer
    {
        private static readonly Func<object, Func<Type, Type>, TypeConverter, KeyValuePair<string, object>[]>[] Factories =
        {
            ItemizeDictionary,
            ItemizeList,
            ItemizeHashSet,
            //CreateArray
        };

        public static KeyValuePair<string, object>[] ItemizeCollection(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            return
                Factories
                    .Select(factory => factory(settings, mapDataType, converter))
                    .FirstOrDefault(collection => collection != null);

        }

        public static KeyValuePair<string, object>[] ItemizeDictionary(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsDictionary()) { return null; }

            var valueType = settings.GetType().GetGenericArguments()[1];
            var dataType = mapDataType(valueType);

            var dictionary = (IDictionary)settings;

            var result = dictionary.Keys.Cast<object>().Select(
                key => new KeyValuePair<string, object>(
                    key: (string)converter.Convert(key, typeof(string)),
                    value: converter.Convert(dictionary[key], dataType)
            )).ToArray();

            return result;
        }

        public static KeyValuePair<string, object>[] ItemizeList(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsList()) { return null; }

            var valueType = settings.GetType().GetGenericArguments()[0];
            var dataType = mapDataType(valueType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select(x => new KeyValuePair<string, object>(key: null, value: converter.Convert(x, dataType)))
                .ToArray();

            return result;
        }

        public static KeyValuePair<string, object>[] ItemizeHashSet(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsHashSet()) { return null; }

            var valueType = settings.GetType().GetGenericArguments()[0];
            var dataType = mapDataType(valueType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select(x => new KeyValuePair<string, object>(key: null, value: converter.Convert(x, dataType)))
                .ToArray();

            return result;
        }

        public static KeyValuePair<string, object>[] CreateArray(object settings, Func<Type, Type> mapDataType, TypeConverter converter)
        {
            if (!settings.GetType().IsArray)
            {
                return null;
            }

            var valueType = settings.GetType().GetGenericArguments()[0];
            var dataType = mapDataType(valueType);

            var result =
                ((IEnumerable)settings).Cast<object>()
                .Select(x => new KeyValuePair<string, object>(key: null, value: converter.Convert(x, dataType)))
                .ToArray();

            return result;
        }
    }
}
