using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    /// <summary>
    /// Provides general utilities.
    /// </summary>
    internal static class Utilities
    {
        internal static MemberInfo GetMemberInfo<TField>(Expression<Func<TField>> expression)
        {
            Debug.Assert(expression != null);

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression.Member;
        }

        internal static IEnumerable<SettingInfo> GetSettingInfos(Type currentType)
        {
            var fields = currentType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);

            foreach (var field in fields)
            {
                yield return new SettingInfo(field);
            }

            var settingInfos = currentType
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Public)
                .Where(t => t.GetCustomAttribute<IgnoreAttribute>() == null)
                .SelectMany(GetSettingInfos);

            foreach (var settingInfo in settingInfos)
            {
                yield return settingInfo;
            }
        }

        internal static SettingInfo FindSettingInfo(Type configType, string settingPath)
        {
            return GetSettingInfos(configType).SingleOrDefault(si => si.SettingPath == settingPath);
        }
    }


}
