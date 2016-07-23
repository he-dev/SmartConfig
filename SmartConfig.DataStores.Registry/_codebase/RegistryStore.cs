using System;
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

        private readonly IDictionary<Type, RegistryValueKind> _typeToRegistryValueKindMap = new Dictionary<Type, RegistryValueKind>
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

        public List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces)
        {
            path.Validate(nameof(path)).IsNotNull();

            path = new RegistryPath(path);

            var subKeyName = Path.Combine(_baseSubKeyName, path.SettingNamespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, false))
            {
                var value = subKey?.GetValue(path.SettingNameWithValueKey);
                return new List<Setting>
                {
                    new Setting
                    {
                        Name = path.ToString(),
                        Value = value
                    }
                };
            }
        }

        public int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value)
        {
            path.Validate(nameof(path)).IsNotNull();

            path = new RegistryPath(path);

            // check value type
            var registryValueKind = RegistryValueKind.None;
            if (!_typeToRegistryValueKindMap.TryGetValue(value.GetType(), out registryValueKind))
            {
                throw new UnsupportedTypeException
                {
                    ValueType = value.GetType().Name,
                    SettingName = path.ToString()
                };
            }

            var subKeyName = Path.Combine(_baseSubKeyName, path.SettingNamespace);
            using (var subKey = _baseKey.OpenSubKey(subKeyName, true) ?? _baseKey.CreateSubKey(subKeyName))
            {
                if (subKey == null) throw new RegistryKeyException($"Could not open/create sub key: '{subKeyName}'.");
                subKey.SetValue(path.SettingName, value, registryValueKind);
            }
            return 1;
        }

        public int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            return settings.Sum(setting => SaveSetting(setting.Key, namespaces, setting.Value));
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
