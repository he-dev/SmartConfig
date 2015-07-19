using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig
{
    public delegate bool TryGetValueCallback(string key, out string value);

    public delegate void UpdateValueCallback(string key, string value);

    public static class SmartConfig
    {
        public static readonly StringConverters StringConverters = new StringConverters();

        public static TryGetValueCallback TryGetValue { get; set; }

        public static UpdateValueCallback UpdateValue { get; set; }        

        //static SmartConfig()
        //{
        //    var smartConfigTypes = 
        //        AppDomain
        //        .CurrentDomain
        //        .GetAssemblies()
        //        .SelectMany(a => a.GetTypes())
        //        .Where(t => t.GetCustomAttribute<SmartConfigAttribute>() != null)
        //        .ToList() ;

        //    //smartConfigTypes[0].GetNestedType("SmartConfig").GetFields();

        //}

        //public static EventHandler<>     

        public static void Load<T>()
        {            
            var configName = GetConfigName(typeof(T));
            RecursiveLoad(typeof(T), configName);
        }

        private static void RecursiveLoad(Type type, string key)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                var subkey = string.IsNullOrEmpty(key) ? field.Name : key + "." + field.Name;

                var value = string.Empty;
                if (TryGetValue(subkey, out value))
                {
                    var typedValue = StringParser.Parse(value, field.FieldType, CultureInfo.InvariantCulture);
                    field.SetValue(null, typedValue);
                }
                //var value = field.GetValue(null);
            }

            var nestedTypes = type.GetNestedTypes();
            foreach (var nestedType in nestedTypes)
            {
                var subkey = string.IsNullOrEmpty(key) ? nestedType.Name : key + "." + nestedType.Name;
                RecursiveLoad(nestedType, subkey);
            }
        }

        public static void Update<T>(Expression<Func<T>> expression, T value) where T : IConvertible
        {
            var key = ConfigKey.Create(expression);

            object cacheItem = null;

            // Try get value from cache.
            if (false)//Cache.TryGetValue(key, out cacheItem))
            {
                //return (cacheItem as CacheItem<T>).Value;
            }//
            // Value not in cache yet.
            else
            {
                //var newCacheItem = new CacheItem<T>();

                string stringValue = null;

                // Got value from source.
                if (TryGetValue(key, out stringValue))
                {
                    newCacheItem.Value = value.ToString(CultureInfo.InvariantCulture); 
                    // StringConverters.GetStringConverter<T>().ConvertTo(stringValue, Properties.CultureInfo);
                }
                // Fallback to default.
                else
                {
                    var memberInfo = ConfigKey.GetMemberInfo(expression);
                    var defaultValue = expression.Compile()();
                    //newCacheItem.Value = defaultValue;
                }

                //Cache[key] = newCacheItem;
                //return newCacheItem.Value;
            }
        }

        //public static void SetValue<T>(Expression<Func<T>> expression, T value)
        //{
        //    var key = ConfigKey.Create(expression);
        //    var stringValue = StringConverters.GetStringConverter<T>().ConvertFrom(value, Properties.CultureInfo);

        //    SetValueCallback(key, stringValue);

        //    Cache.Remove(key);
        //}        

        internal static string GetConfigName(Type smartConfigType)
        {
            var smartConfigAttribute = smartConfigType.GetCustomAttribute<SmartConfigAttribute>();
            if (smartConfigAttribute.MulticonfigEnabled)
            {
                var configName = GetConfigName(smartConfigType);

                // Remove "Config(uration)" suffix.
                configName = Regex.Replace(smartConfigType.Name, "Config(uration)?$", string.Empty);
                return configName;
            }
            return string.Empty;
        }


        internal static class Properties
        {
            public static CultureInfo CultureInfo = CultureInfo.InvariantCulture;
        }
    }
}
