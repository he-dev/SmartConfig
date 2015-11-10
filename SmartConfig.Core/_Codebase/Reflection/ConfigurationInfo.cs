using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Reflection
{
    internal class ConfigurationInfo
    {
        public ConfigurationInfo(Type configType)
        {
            if (configType == null)
            {
                throw new ArgumentNullException(nameof(configType));
            }

            if (!configType.IsStatic())
            {
                throw new ConfigTypeNotStaticException { ConfigTypeFullName = configType.FullName };
            }

            var smartConfigAttribute = configType.GetCustomAttribute<SmartConfigAttribute>();
            if (smartConfigAttribute == null)
            {
                throw new SmartConfigAttributeMissingException { ConfigTypeFullName = configType.FullName };
            }

            ConfigType = configType;
            ConfigName = smartConfigAttribute.Name;
            Properties = new ConfigurationProperties(configType);
            SettingInfos = GetSettingInfos(configType).ToDictionary(x => x.Property);

        }

        public Type ConfigType { get; }

        public string ConfigName { get; }

        public ConfigurationProperties Properties { get; }

        public bool HasCustomName => !string.IsNullOrEmpty(ConfigName);

        public IDictionary<PropertyInfo, SettingInfo> SettingInfos { get; }

        private IEnumerable<SettingInfo> GetSettingInfos(Type type, IEnumerable<string> path = null)
        {
            path = path ?? (HasCustomName ? new[] { ConfigName } : new string[] { });

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);

            foreach (var property in properties)
            {
                yield return new SettingInfo(this, property, path.Concat(new[] { property.Name }), Properties.CustomKeys);
            }

            var canSelectType = new Func<Type, bool>(t =>
            {
                if (t.GetCustomAttribute<IgnoreAttribute>() != null)
                {
                    return false;
                }

                if (t.Name == "Properties" && t.DeclaringType == ConfigType)
                {
                    return false;
                }

                return true;
            });

            var settingInfos = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Public)
                .Where(canSelectType)
                .SelectMany(nt => GetSettingInfos(nt, path.Concat(new[] { nt.Name })));

            foreach (var settingInfo in settingInfos)
            {
                yield return settingInfo;
            }
        }

        public SettingInfo FindSettingInfo(Type configType, string settingPath)
        {
            return GetSettingInfos(configType).SingleOrDefault(si => si.SettingPath == settingPath);
        }

        public static IEnumerable<Type> GetDeclaringTypes(MemberInfo memberInfo)
        {
            var type = memberInfo.DeclaringType;
            while (type != null)
            {
                yield return type;
                if (type.GetCustomAttribute<SmartConfigAttribute>(false) != null)
                {
                    yield break;
                }
                type = type.DeclaringType;
            }

            throw new InvalidOperationException("SmartConfigAttribute not found for");
        }
    }
}
