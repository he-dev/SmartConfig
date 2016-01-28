using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

            var smartConfigAttribute = configurationType.GetCustomAttribute<SmartConfigAttribute>();
            if (smartConfigAttribute == null)
            {
                throw new SmartConfigAttributeMissingException { ConfigTypeFullName = configurationType.FullName };
            }

            ConfigurationType = configurationType;
            ConfigurationPropertyGroup = new ConfigurationPropertyGroup(configurationType);
        }

        public Type ConfigurationType { get; }

        public ConfigurationPropertyGroup ConfigurationPropertyGroup { get; }

        public string ConfigurationName => ConfigurationType.GetCustomAttribute<SettingNameAttribute>()?.SettingName;

        public IEnumerable<SettingInfo> SettingInfos => ConfigurationType.GetSettingInfos(this);

        //public SettingInfo FindSettingInfo(Type configType, string settingPath)
        //{
        //    return GetSettingInfos(configType).SingleOrDefault(si => si.SettingPath == settingPath);
        //}
    }
}
