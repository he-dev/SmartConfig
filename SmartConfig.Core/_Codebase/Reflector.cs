using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig
{
    internal static class Reflector
    {
        /// <summary>
        /// Checks if the specified memberInfo is static.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStatic(this Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            return type.IsAbstract && type.IsSealed;
        }

        public static string GetSettingNameOrMemberName(this MemberInfo member)
        {
            if (member == null) { throw new ArgumentNullException(nameof(member)); }

            var settingName = member.GetCustomAttribute<SettingNameAttribute>();
            return settingName != null ? settingName.SettingName : member.Name;
        }

        public static bool IsSmartConfigType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();
            return smartConfigAttribute != null;
        }

        public static List<string> GetSettingPath(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var path = new List<string> { propertyInfo.GetSettingNameOrMemberName() };

            var type = propertyInfo.DeclaringType;

            while (type != null && !type.IsSmartConfigType())
            {
                path.Add(type.GetSettingNameOrMemberName());

                type = type.DeclaringType;
                if (type == null)
                {
                    throw new SmartConfigAttributeNotFoundException
                    {
                        AffectedProperty = propertyInfo.PropertyType.FullName
                    };
                }
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Gets configuration types without ignored types and SmartConfig properties type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetConfigurationTypes(this Type type, List<Type> result = null)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            if (!type.IsStatic())
            {
                throw new TypeNotStaticException { Type = type.FullName };
            }

            result = result ?? new List<Type> { type };

            var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Public);
            result.AddRange(nestedTypes);

            foreach (var nestedType in nestedTypes)
            {
                nestedType.GetConfigurationTypes(result);
            }

            return result.Where(t => !t.HasAttribute<IgnoreAttribute>());
        }

        /// <summary>
        /// Gets Settings without ignored properties.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnumerable<Setting> GetSettings(this Configuration configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

            var types = configuration.Type.GetConfigurationTypes();

            var settingInfos = types
                .Select(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .SelectMany(sis => sis)
                .Where(p => !p.HasAttribute<IgnoreAttribute>())
                .Select(p => new Setting(p, configuration));

            return settingInfos;
        }        

        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            if (memberInfo == null) { throw new ArgumentNullException(nameof(memberInfo)); }

            return memberInfo.GetCustomAttributes(typeof(T), false).Any();
        }

        public static IEnumerable<string> GetSettingKeyNames<TSetting>() where TSetting : BasicSetting
        {
            yield return nameof(BasicSetting.Name);

            var settingType = typeof(TSetting);

            var isBasicSettingType = settingType == typeof(BasicSetting);
            if (isBasicSettingType)
            {
                yield break;
            }

            var customKeyNames = settingType
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name)
                    .OrderBy(n => n);

            foreach (var customKeyName in customKeyNames)
            {
                yield return customKeyName;
            }
        }

#if NET40

        public static IEnumerable<T> GetCustomAttributes<T>(this Type memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof (T), inherit).Cast<T>();
        }


        public static TAttribute GetCustomAttribute<TAttribute>(this Type memberInfo, bool inherit = false) where TAttribute : Attribute
        {
            return memberInfo.GetCustomAttributes<TAttribute>(inherit).SingleOrDefault();
        }

#endif

        //public static bool IsNullable(this Type memberInfo)
        //{
        //    var isNullable =
        //        memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        //    return isNullable;
        //}

        //public static bool IsIEnumerable(this Type memberInfo)
        //{
        //    var isIEnumerable =
        //        memberInfo != typeof(string)
        //        && memberInfo.GetInterfaces()
        //        .Any(t => t.IsGenericType && memberInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        //    return isIEnumerable;
        //}

        //public static bool IsList(this Type memberInfo)
        //{
        //    var isList =
        //        memberInfo != typeof(string)
        //        && memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(List<>);
        //    return isList;
        //}

        //public static bool IsDictionary(this Type memberInfo)
        //{
        //    var isList =
        //        memberInfo != typeof(string)
        //        && memberInfo.IsGenericType
        //        && memberInfo.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        //    return isList;
        //}



        //        public static T GetCustomAttribute<T>(this Type memberInfo, bool inherit = false) where T : Attribute
        //        {
        //#if NET40
        //            return (T)memberInfo.GetCustomAttributes(typeof(T), inherit).SingleOrDefault();
        //#else
        //            return memberInfo.GetCustomAttributes(inherit).OfType<T>().SingleOrDefault();
        //#endif
        //        }
    }
}
