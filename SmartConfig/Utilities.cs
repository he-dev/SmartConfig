using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the config name from the config type if specified.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string ConfigName(Type type)
        {
            var smartConfigAttribute = type.CustomAttribute<SmartConfigAttribute>();
            if (smartConfigAttribute == null)
            {
                throw new InvalidOperationException("Type is not marked with SmartConfigAttribute.");
            }

            //var configName = type.FullName.Split('.').Last();

            // Remove "Config(uration)" suffix:
            //configName = Regex.Replace(type.Name, "Config(uration)?$", string.Empty);

            return string.IsNullOrEmpty(smartConfigAttribute.Name) ? string.Empty : smartConfigAttribute.Name;
        }

        internal static Type GetConfigType(MemberInfo memberInfo)
        {
            if (memberInfo.ReflectedType.HasAttribute<SmartConfigAttribute>())
            {
                return memberInfo.ReflectedType;
            }

            var type = memberInfo.DeclaringType;
            while (type != null)
            {
                if (type.HasAttribute<SmartConfigAttribute>())
                {
                    return type;
                }
                type = type.DeclaringType;
            }

            throw new SmartConfigTypeNotFoundException("Neither the specified type nor any declaring type is marked with the SmartConfigAttribute.");
        }

        internal static ConfigFieldInfo GetConfigFieldInfo(MemberInfo memberInfo)
        {
            var path = new List<string> { memberInfo.Name };

            var type = memberInfo.DeclaringType;
            while (type != null)
            {
                var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();
                if (smartConfigAttribute != null)
                {
                    path.Add(smartConfigAttribute.Name);
                    var result = new ConfigFieldInfo()
                    {
                        SmartConfigType = type,
                        Version = smartConfigAttribute.Version,
                        Name = ConfigElementName.Combine(path, true),
                        Constraints = ((FieldInfo)memberInfo).GetCustomAttributes<ValueConstraintAttribute>()
                    };
                    return result;
                }
                path.Add(type.Name);
                type = type.DeclaringType;
            }

            throw new SmartConfigTypeNotFoundException("Neither the specified type nor any declaring type is marked with the SmartConfigAttribute.");
        }

        internal static MemberInfo GetMemberInfo<TField>(Expression<Func<TField>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression.Member;
        }

    }
}
