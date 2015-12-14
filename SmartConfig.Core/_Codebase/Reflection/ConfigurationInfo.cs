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
            ConfigurationProperties = new ConfigurationProperties(configurationType);
            SettingInfos = GetSettingInfos(configurationType).ToDictionary(x => x.Property);
        }

        public Type ConfigurationType { get; }

        public ConfigurationProperties ConfigurationProperties { get; }

        public bool HasCustomName => !string.IsNullOrEmpty(ConfigurationProperties.Name);

        public IDictionary<PropertyInfo, SettingInfo> SettingInfos { get; }

        private IEnumerable<SettingInfo> GetSettingInfos(Type type, IEnumerable<string> path = null)
        {
            if (!type.IsStatic())
            {
                throw new TypeNotStaticException { TypeFullName = type.FullName };
            }

            path = path ?? (HasCustomName ? new[] { ConfigurationProperties.Name } : new string[] { });

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.GetCustomAttribute<IgnoreAttribute>() == null);

            foreach (var property in properties)
            {
                var subPath = path.Concat(new[] { property.Name });
                yield return new SettingInfo(this, property, subPath, ConfigurationProperties.CustomKeys);
            }

            var typeConditions = new Func<Type, bool>[]
            {
                t => t.GetCustomAttribute<IgnoreAttribute>() == null,
                t => t.Name != "Properties",
            };

            var canGetType = new Func<Type, bool>(t => typeConditions.All(typeCondition => typeCondition(t)));

            var settingInfos = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Public)
                .Where(canGetType)
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
