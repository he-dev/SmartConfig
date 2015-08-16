﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public static class Utilities
    {
        /// <summary>
        /// Gets the config name from the config type if specified.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string ConfigName(Type type)
        {
            var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>();
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

            throw new SmartConfigTypeNotFoundException();
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

        //public static Dictionary<string, string> CombineDictionaries(IDictionary<string, string> dic1, IDictionary<string, string> dic2)
        //{
        //    return dic1.Concat(dic2).ToDictionary(x => x.Key, x => x.Value);
        //}
    }
}
