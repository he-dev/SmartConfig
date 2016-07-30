#if DEBUG
//#define DISABLE_DELETE_VALUE
//#define DISABLE_SET_VALUE
#endif
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryStore : IDataStore
    {
        private readonly RegistryKey _baseKey;
        private readonly string _baseSubKeyName;

        private readonly IReadOnlyDictionary<Type, RegistryValueKind> _registryValueKindMap = new Dictionary<Type, RegistryValueKind>
        {
            { typeof(string), RegistryValueKind.String },
            { typeof(int), RegistryValueKind.DWord },
            { typeof(byte[]), RegistryValueKind.Binary },
        };

        public RegistryStore(RegistryKey baseKey, string subKey)
        {
            baseKey.Validate(nameof(baseKey)).IsNotNull();
            subKey.Validate(nameof(subKey)).IsNotNullOrEmpty();

            _baseKey = baseKey;
            _baseSubKeyName = subKey;
        }

        public Type MapDataType(Type settingType)
        {
            if (settingType == typeof(int)) return typeof(int);
            if (settingType == typeof(byte[])) return typeof(byte[]);
            return typeof(string);
        }

        public List<Setting> GetSettings(Setting setting)
        {
            var registryPath = new RegistryPath(setting.Name);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SettingNamespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, false))
            {
                if (subKey == null) { return new List<Setting>(); }

                var settings =
                    subKey.GetValueNames()
                        .Where(x => RegistryPath.Parse(x).IsLike(registryPath.FullName))
                        .Select(x => new Setting
                        {
                            Name = x,
                            Value = subKey.GetValue(x)
                        })
                        .ToList();

                return settings;
            }
        }

        public int SaveSetting(Setting setting)
        {
            return SaveSettings(new[] { setting });
        }

        public int SaveSettings(IReadOnlyCollection<Setting> settings)
        {
            if (!settings.Any())
            {
                return 0;
            }

            var settingGroups = settings.GroupBy(x => x.Name.FullName).ToList();

            // Before starting to delete/set keys check if all settings have valid types.
            var unsupportedSettings =
                settingGroups.Where(sg => !_registryValueKindMap.ContainsKey(sg.First().Value.GetType()))
                .ToList();

            if (unsupportedSettings.Any())
            {
                throw new UnsupportedTypeException(
                    unsupportedSettings.Select(x => x.First().Name.FullName),
                    _registryValueKindMap.Select(x => x.Key));
            }

            // Delete all settings first.
            foreach (var sg in settingGroups)
            {
                // We the first element to delete other elements that are alike and to get the setting type.
                var s0 = sg.First();
                var rp = new RegistryPath(s0.Name);

                var subKeyName = Path.Combine(_baseSubKeyName, rp.SettingNamespace);
                using (var subKey = _baseKey.OpenSubKey(subKeyName, true) ?? _baseKey.CreateSubKey(subKeyName))
                {
                    if (subKey == null) { continue; }

                    var valueNames2Delete =
                        subKey.GetValueNames()
                        .Where(x => RegistryPath.Parse(x).IsLike(rp))
                        .ToList();

                    foreach (var valueName in valueNames2Delete)
                    {
#if !DISABLE_DELETE_VALUE
                        subKey.DeleteValue(valueName);
#endif
                    }

                    // Get registry value kind from the first setting.
                    var registryValueKind = _registryValueKindMap[s0.Value.GetType()];

                    // Save all group settings.
                    foreach (var s in sg)
                    {
                        var registryPath = new RegistryPath(s.Name);
#if !DISABLE_SET_VALUE
                        subKey.SetValue(registryPath.SettingNameEx, s.Value, registryValueKind);
#endif
                    }
                }
            }

            return settings.Count;
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
