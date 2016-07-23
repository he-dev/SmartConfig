using System;
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
    internal class CollectionFactory
    {
        private static readonly Func<IReadOnlyCollection<Setting>, Type, TypeConverter, object>[] Factories =
        {
            CreateDictionary,
            CreateList,
            CreateHashSet,
            CreateArray
        };

        public static object CreateCollection(IReadOnlyCollection<Setting> settings, Type collectionType, TypeConverter converter)
        {
            return
                Factories
                    .Select(factory => factory(settings, collectionType, converter))
                    .FirstOrDefault(collection => collection != null);

        }

        public static object CreateDictionary(IReadOnlyCollection<Setting> settings, Type collectionType, TypeConverter converter)
        {
            if (!collectionType.IsDictionary()) { return null; }

            var keyType = collectionType.GetGenericArguments()[0];
            var valueType = collectionType.GetGenericArguments()[1];

            var dictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);

            foreach (var setting in settings)
            {
                dictionary.Add(setting.Name.ValueKey, converter.Convert(setting.Value, valueType));
            }

            return dictionary;
        }

        public static object CreateList(IReadOnlyCollection<Setting> settings, Type collectionType, TypeConverter converter)
        {
            if (!collectionType.IsList()) { return null; }

            var valueType = collectionType.GetGenericArguments()[0];

            var listType = typeof(List<>).MakeGenericType(valueType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var setting in settings)
            {
                list.Add(converter.Convert(setting.Value, valueType));
            }

            return list;
        }

        public static object CreateHashSet(IReadOnlyCollection<Setting> settings, Type collectionType, TypeConverter converter)
        {
            if (!collectionType.IsHashSet()) { return null; }

            var valueType = collectionType.GetGenericArguments()[0];

            var hashSetType = typeof(HashSet<>).MakeGenericType(valueType);
            var hashSet = Activator.CreateInstance(hashSetType);
            var addMethod = hashSetType.GetMethod("Add");

            foreach (var setting in settings)
            {
                addMethod.Invoke(hashSet, new[] { converter.Convert(setting.Value, valueType) });
            }

            return hashSet;
        }

        public static object CreateArray(IReadOnlyCollection<Setting> settings, Type collectionType, TypeConverter converter)
        {
            if (!collectionType.IsArray) { return null; }

            var elementType = collectionType.GetElementType();
            var array = Array.CreateInstance(elementType, settings.Count);

            var i = 0;
            foreach (var setting in settings)
            {
                var element = converter.Convert(setting.Value, elementType);
                array.SetValue(element, i++);
            }

            return array;
        }
    }
}
