using System;
using System.Collections;
using System.Collections.Generic;
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
    public static class Utilities
    {
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

    public class KeyMembers : List<string>
    {
        public static KeyMembers From<TSetting>() where TSetting : Setting
        {
            var keyMembers = new KeyMembers()
            {
                KeyNames.DefaultKeyName
            };

            var currentType = typeof(TSetting);

            var isCustomType = currentType != typeof(Setting);
            if (!isCustomType)
            {
                return keyMembers;
            }

            var propertyNames =
                currentType
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name)
                    .OrderBy(n => n);
            keyMembers.AddRange(propertyNames);

            return keyMembers;
        }       
    }
}
