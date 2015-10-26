using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal class ConfigReflector : IConfigReflector
    {
        //internal static MemberInfo GetMemberInfo<TField>(Expression<Func<TField>> expression)
        //{
        //    Debug.Assert(expression != null);

        //    var memberExpression = expression.Body as MemberExpression;
        //    if (memberExpression == null)
        //    {
        //        var unaryExpression = expression.Body as UnaryExpression;
        //        memberExpression = unaryExpression.Operand as MemberExpression;
        //    }

        //    return memberExpression.Member;
        //}

        public IEnumerable<SettingInfo> GetSettingInfos(Type currentType)
        {
            //var fields = currentType
            //    .GetFields(BindingFlags.Public | BindingFlags.Static)
            //    .Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);

            var properties = currentType
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);

            foreach (var property in properties)
            {
                yield return new SettingInfo(property);
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

        public SettingInfo FindSettingInfo(Type configType, string settingPath)
        {
            return GetSettingInfos(configType).SingleOrDefault(si => si.SettingPath == settingPath);
        }
    }
}
