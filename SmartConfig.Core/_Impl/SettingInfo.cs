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
    [DebuggerDisplay("ConfigType.Name = {ConfigType.Name} SettingPath = \"{SettingPath}\" IsInternal = \"{IsInternal}\"")]
    public class SettingInfo
    {
        internal SettingInfo(Type configType, FieldInfo fieldInfo, string settingName)
        {
            var smartConfigAttribute = configType.GetCustomAttribute<SmartConfigAttribute>(false);
            ConfigType = configType;
            ConfigName = smartConfigAttribute.Name;
            FieldInfo = fieldInfo;
            SettingPath = new SettingPath(ConfigName, settingName);
        }

        internal SettingInfo(Type configType, string settingName) : this(configType, null, settingName)
        {
        }

        internal SettingInfo(MemberInfo member)
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
                    SettingPath = new SettingPath(((IEnumerable<string>)path).Reverse());
                }
                path.Add(type.Name);
                type = type.DeclaringType;

                if (type == null && smartConfigAttribute == null)
                {
                    throw new SmartConfigTypeNotFoundException();
                }
            }
        }

        #region Config Info

        public Type ConfigType { get; private set; }
        public string ConfigName { get; private set; }

        #endregion

        #region FieldInfo Info

        public FieldInfo FieldInfo { get; private set; }
        public SettingPath SettingPath { get; private set; }

        public IEnumerable<ConstraintAttribute> SettingConstraints
        {
            get { return FieldInfo == null ? Enumerable.Empty<ConstraintAttribute>() : FieldInfo.Contraints(); }
        }

        #endregion

        internal bool IsInternal { get; private set; }
    }
}
