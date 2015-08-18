using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class SettingInfo
    {
        private SettingInfo(MemberInfo member)
        {
            FieldInfo = (FieldInfo)member;

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
                    //ConfigVersion = smartConfigAttribute.Version;
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

        internal static SettingInfo From(MemberInfo member)
        {
            return new SettingInfo(member);
        }

        internal static SettingInfo From<TField>(Expression<Func<TField>> expression)
        {
            return new SettingInfo(Utilities.GetMemberInfo(expression));
        }

        #region Config Info

        public Type ConfigType { get; private set; }
        public string ConfigName { get; private set; }

        #endregion

        #region FieldInfo Info

        public FieldInfo FieldInfo { get; private set; }
        public string FieldPath { get; private set; }
        public IEnumerable<ConstraintAttribute> FieldConstraints { get; private set; }
        public object FieldValue { get { return FieldInfo.GetValue(null); } }

        #endregion
    }    
}
