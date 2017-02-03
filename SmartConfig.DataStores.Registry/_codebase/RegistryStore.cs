#if DEBUG
//#define DISABLE_DELETE_VALUE
//#define DISABLE_SET_VALUE
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Reusable;
using Reusable.Fuse;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryStore : DataStore
    {
        private readonly RegistryKey _baseKey;
        private readonly string _baseSubKeyName;

        private readonly IReadOnlyDictionary<Type, RegistryValueKind> _registryValueKinds = new Dictionary<Type, RegistryValueKind>
        {
            { typeof(string), RegistryValueKind.String },
            { typeof(int), RegistryValueKind.DWord },
            { typeof(byte[]), RegistryValueKind.Binary },
        };

        public RegistryStore(RegistryKey baseKey, string subKey)
            : base(new[]
            {
                typeof(int),
                typeof(byte[]),
                typeof(string)
            })
        {
            baseKey.Validate(nameof(baseKey)).IsNotNull();
            subKey.Validate(nameof(subKey)).IsNotNullOrEmpty();

            _baseKey = baseKey;
            _baseSubKeyName = subKey;
        }

        public override IEnumerable<Setting> GetSettings(Setting setting)
        {
            var registryPath = new RegistryUrn(setting.Name);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.Namespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, false))
            {
                if (subKey == null)
                {
                    throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName);
                }

                var settings =
                    subKey.GetValueNames()
                        .Where(valueName => RegistryUrn.Parse(valueName).IsLike(registryPath.WeakFullName))
                        .Select(valueName => new Setting
                        {
                            Name = SettingPath.Parse(valueName),
                            Value = subKey.GetValue(valueName)
                        })
                        .ToList();

                return settings;
            }
        }

        public override int SaveSettings(IEnumerable<Setting> settings)
        {
            var groups = settings.GroupBy(x => x.Name.WeakFullName).ToList();

            foreach (var group in groups)
            {
                var groupDeleted = false;
                foreach (var setting in group)
                {
                    var registryValueKind = RegistryValueKind.None;
                    if (!_registryValueKinds.TryGetValue(setting.Value.GetType(), out registryValueKind))
                    {
                        throw new InvalidTypeException(setting.Value.GetType(), SupportedTypes);
                    }

                    var subKeyName = Path.Combine(_baseSubKeyName, setting.Name.Namespace);
                    using (var subKey = _baseKey.OpenSubKey(subKeyName, true) ?? _baseKey.CreateSubKey(subKeyName))
                    {
                        if (subKey == null)
                        {
                            throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName);
                        }

                        if (!groupDeleted)
                        {
                            var valueNames = subKey
                                .GetValueNames()
                                .Where(x => RegistryUrn.Parse(x).IsLike(setting.Name))
                                .ToList();

                            foreach (var valueName in valueNames)
                            {
#if !DISABLE_DELETE_VALUE
                                subKey.DeleteValue(valueName);
#endif
                            }

                            groupDeleted = true;
                        }

                        var registryUrn = new RegistryUrn(setting.Name);
#if !DISABLE_SET_VALUE
                        subKey.SetValue(registryUrn.StrongName, setting.Value, registryValueKind);
#endif
                    }
                }

            }

            return groups.Sum(x => x.Count());
        }

        public static RegistryStore CreateForCurrentUser(string subRegistryKey)
        {
            return new RegistryStore(Microsoft.Win32.Registry.CurrentUser, subRegistryKey);
        }

        public static RegistryStore CreateForClassesRoot(string subRegistryKey)
        {
            return new RegistryStore(Microsoft.Win32.Registry.ClassesRoot, subRegistryKey);
        }

        public static RegistryStore CreateForCurrentConfig(string subRegistryKey)
        {
            return new RegistryStore(Microsoft.Win32.Registry.CurrentConfig, subRegistryKey);
        }

        public static RegistryStore CreateForLocalMachine(string subRegistryKey)
        {
            return new RegistryStore(Microsoft.Win32.Registry.LocalMachine, subRegistryKey);
        }

        public static RegistryStore CreateForUsers(string subRegistryKey)
        {
            return new RegistryStore(Microsoft.Win32.Registry.Users, subRegistryKey);
        }
    }
}
