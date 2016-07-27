using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using SmartConfig.Collections;
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
            // we need one setting to delete all subkeys like it in case it's a collection and to check its value type
            var firstSetting = settings.FirstOrDefault();
            if (string.IsNullOrEmpty(firstSetting?.Name.FullName))
            {
                return 0;
            }

            var registryValueKind = RegistryValueKind.None;
            if (!_registryValueKindMap.TryGetValue(firstSetting.Value.GetType(), out registryValueKind))
            {
                throw new UnsupportedTypeException
                {
                    SettingName = firstSetting.Name.FullNameEx,
                    ValueType = firstSetting.Value.GetType().Name
                };
            }

            var registryPath = new RegistryPath(firstSetting.Name.FullName);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SettingNamespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, true) ?? _baseKey.CreateSubKey(subKeyName))
            {
                if (subKey == null)
                {
                    throw new RegistryKeyException($"Could not open/create sub key: '{subKeyName}'.");
                }

                var valueNames2Delete =
                    subKey.GetValueNames()
                        .Where(x => RegistryPath.Parse(x).IsLike(registryPath.FullName))
                        .ToList();

                foreach (var valueName in valueNames2Delete)
                {
                    subKey.DeleteValue(valueName);
                }

                foreach (var setting in settings)
                {
                    registryPath = new RegistryPath(setting.Name);
                    subKey.SetValue(registryPath.FullNameEx, setting.Value, registryValueKind);
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

    public class UnsupportedTypeException : FormattableException
    {
        public string ValueType { get; internal set; }
        public string SettingName { get; internal set; }
    }

    public class RegistryKeyException : FormattableException
    {
        public RegistryKeyException(string message) : base(message) { }
        //public string ValueType { get; internal set; }
        //public string SettingName { get; internal set; }
    }
}
