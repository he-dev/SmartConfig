using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Collections;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Reflection
{
    internal class ConfigurationInfo
    {
        public ConfigurationInfo(Type configurationType)
        {
            if (configurationType == null)
            {
                throw new ArgumentNullException(nameof(configurationType));
            }

            if (!configurationType.IsStatic())
            {
                throw new TypeNotStaticException { TypeFullName = configurationType.FullName };
            }

            if (!configurationType.HasAttribute<SmartConfigAttribute>())
            {
                throw new SmartConfigAttributeMissingException { ConfigurationTypeFullName = configurationType.FullName };
            }

            ConfigurationType = configurationType;
            ConfigurationPropertyGroup = new ConfigurationPropertyGroup(configurationType);
        }

        public Type ConfigurationType { get; }

        public ConfigurationPropertyGroup ConfigurationPropertyGroup { get; }

        public string ConfigurationName => ConfigurationType.GetCustomAttribute<SettingNameAttribute>()?.SettingName;

        public IEnumerable<SettingInfo> SettingInfos => this.GetSettingInfos();

        //public SettingInfo FindSettingInfo(DeclaringTypeName configType, string settingPath)
        //{
        //    return GetSettingInfos(configType).SingleOrDefault(si => si.SettingPath == settingPath);
        //}
    }
}
