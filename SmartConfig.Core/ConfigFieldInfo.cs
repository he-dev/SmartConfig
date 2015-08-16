using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class ConfigFieldInfo
    {
        private ConfigFieldInfo(MemberInfo member)
        {
            Field = (FieldInfo)member;

            var path = new List<string> { member.Name };

            var type = member.DeclaringType;
            while (type != null)
            {
                var smartConfigAttribute = type.GetCustomAttribute<SmartConfigAttribute>(false);
                var isSmartConfigType = smartConfigAttribute != null;
                if (isSmartConfigType)
                {
                    path.Add(smartConfigAttribute.Name);
                    ConfigType = type;
                    ConfigName = smartConfigAttribute.Name;
                    ConfigVersion = smartConfigAttribute.Version;
                    FieldPath = SmartConfig.FieldPath.Combine(((IEnumerable<string>)path).Reverse());
                    FieldConstraints = ((FieldInfo)member).GetCustomAttributes<ConstraintAttribute>(false);
                }
                path.Add(type.Name);
                type = type.DeclaringType;

                if (type == null && smartConfigAttribute == null)
                {
                    throw new SmartConfigTypeNotFoundException();
                }
            }
        }

        internal static ConfigFieldInfo From(MemberInfo member)
        {
            return new ConfigFieldInfo(member);
        }

        internal static ConfigFieldInfo From<TField>(Expression<Func<TField>> expression)
        {
            return new ConfigFieldInfo(Utilities.GetMemberInfo(expression));
        }

        #region Config Info

        public Type ConfigType { get; private set; }
        public string ConfigName { get; private set; }
        public string ConfigVersion { get; private set; }

        #endregion

        #region Field Info

        public FieldInfo Field { get; private set; }
        public string FieldPath { get; private set; }
        public IEnumerable<ConstraintAttribute> FieldConstraints { get; private set; }

        #endregion
    }

    //class ConfigInfo
    //{
    //    public Type ConfigType { get; private set; }
    //    public string ConfigName { get; private set; }
    //    public IDictionary<string, string> ConfigKeys { get; private set; }
    //}

    //class ConfigField
    //{
    //    private FieldInfo _fieldInfo;

    //    public string FieldPath { get; private set; }
    //    public IEnumerable<ConstraintAttribute> FieldConstraints { get; private set; }

    //    public static implicit operator FieldInfo(ConfigField configField)
    //    {
    //        return configField._fieldInfo;
    //    }
    //}
}
