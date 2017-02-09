#if DEBUG
#endif
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Reusable;
using Reusable.Fuse;
using SmartConfig.Collections;
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

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            var registryPath = new RegistryPath(setting.Name);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.Namespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, false))
            {
                if (subKey == null)
                {
                    throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName);
                }

                var valueNames = subKey.GetValueNames().Where(x => RegistryPath.Parse(x).IsLike(registryPath));

                foreach (var valueName in valueNames)
                {
                    yield return new Setting
                    {
                        Name = SettingPath.Parse(valueName),
                        Value = subKey.GetValue(valueName)
                    };
                }
            }
        }

        protected override void WriteSettings(ICollection<IGrouping<Setting, Setting>> settings)
        {
            foreach (var group in settings)
            {
                var subKeyName = Path.Combine(_baseSubKeyName, group.Key.Name.Namespace);
                using (var subKey = _baseKey.OpenSubKey(subKeyName, true) ?? _baseKey.CreateSubKey(subKeyName))
                {
                    if (subKey == null)
                    {
                        throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName);
                    }

                    DeleteObsoleteSettings(subKey, group);

                    foreach (var setting in group)
                    {
                        var registryValueKind = RegistryValueKind.None;
                        if (!_registryValueKinds.TryGetValue(setting.Value.GetType(), out registryValueKind))
                        {
                            throw new InvalidTypeException(setting.Value.GetType(), SupportedTypes);
                        }


                        var registryUrn = new RegistryPath(setting.Name);
                        subKey.SetValue(registryUrn.StrongName, setting.Value, registryValueKind);
                    }
                }

            }
        }

        private static void DeleteObsoleteSettings(RegistryKey registryKey, IGrouping<Setting, Setting> settings)
        {
            var obsoleteNames =
                registryKey
                    .GetValueNames()
                    .Where(x => RegistryPath.Parse(x).IsLike(settings.Key.Name))
                    .ToList();

            obsoleteNames.ForEach(registryKey.DeleteValue);
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
