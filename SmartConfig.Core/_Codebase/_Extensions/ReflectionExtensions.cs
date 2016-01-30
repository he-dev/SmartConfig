using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartConfig.Reflection;

namespace SmartConfig
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Checks if the specified memberInfo is static.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }

        public static string GetMemberName(this MemberInfo member)
        {
            var settingName = member.GetCustomAttribute<SettingNameAttribute>();
            return settingName != null ? settingName.SettingName : member.Name;
        }

        public static bool IsSmartConfigType(this Type type)
        {
            var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();
            return smartConfigAttribute != null;
        }

        public static IEnumerable<string> GetPath(this PropertyInfo propertyInfo)
        {
            var path = new List<string> { propertyInfo.GetMemberName() };

            var type = propertyInfo.DeclaringType;

            while (type != null && !type.IsSmartConfigType())
            {
                path.Add(type.GetMemberName());

                type = type.DeclaringType;
            }

            return ((IEnumerable<string>)path).Reverse();
        }

        public static IEnumerable<Type> GetTypes(this Type type, List<Type> result)
        {
            if (!type.IsStatic())
            {
                throw new TypeNotStaticException { TypeFullName = type.FullName };
            }

            result = result ?? new List<Type> { type };

            var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Public);
            result.AddRange(nestedTypes);

            foreach (var nestedType in nestedTypes)
            {
                nestedType.GetTypes(result);
            }

            return result.Where(t => !t.IgnoreClass());
        }

        public static IEnumerable<SettingInfo> GetSettingInfos(this Type type, ConfigurationInfo configuration)
        {
            var types = type.GetTypes(null);

            var settingInfos = 
                types.Select(t =>
                    t.GetProperties(BindingFlags.Public | BindingFlags.Static)
                    .Where(p => !p.IgnoreProperty())
                    .Select(p => new SettingInfo(p, configuration))
                )
                .SelectMany(sis => sis);

            return settingInfos;
        }

        public static bool IgnoreProperty(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<IgnoreAttribute>() != null;
        }

        public static bool IgnoreClass(this Type type)
        {
            // ignore class either if it's marked to be ignored
            // or it's the properties class defined directly under the SmartConfig class
            return
                type.GetCustomAttribute<IgnoreAttribute>() != null
                || (type.GetCustomAttribute<SmartConfigPropertiesAttribute>() != null);
        }

        public static bool HasAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), false).Any();
        }

        //public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        //{
        //    return memberInfo.GetCustomAttributes(typeof(T), false).Any();
        //}

#if NET40

        public static IEnumerable<T> GetCustomAttributes<T>(this SettingType memberInfo, bool inherit = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof (T), inherit).Cast<T>();
        }


        public static TAttribute GetCustomAttribute<TAttribute>(this SettingType memberInfo, bool inherit = false) where TAttribute : Attribute
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



        //        public static T GetCustomAttribute<T>(this SettingType memberInfo, bool inherit = false) where T : Attribute
        //        {
        //#if NET40
        //            return (T)memberInfo.GetCustomAttributes(typeof(T), inherit).SingleOrDefault();
        //#else
        //            return memberInfo.GetCustomAttributes(inherit).OfType<T>().SingleOrDefault();
        //#endif
        //        }
    }
}
