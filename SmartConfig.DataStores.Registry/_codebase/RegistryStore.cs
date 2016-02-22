using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using SmartConfig.Collections;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryStore : DataStore<BasicSetting> //where TSetting : BasicSetting, new()
    {
        private readonly RegistryKey _baseRegistryKey;
        private readonly string _baseSubKeyName;
        private readonly IDictionary<Type, RegistryValueKind> _supportedRegistryValueKinds = new Dictionary<Type, RegistryValueKind>
        {
            { typeof(string), RegistryValueKind.String },
            { typeof(int), RegistryValueKind.DWord },
            { typeof(byte[]), RegistryValueKind.Binary },
        };

        public RegistryStore(RegistryKey baseRegistryKey, string subRegistryKey)
        {
            if (baseRegistryKey == null) { throw new ArgumentNullException(nameof(baseRegistryKey)); }
            if (subRegistryKey == null) { throw new ArgumentNullException(nameof(subRegistryKey)); }

            _baseRegistryKey = baseRegistryKey;
            _baseSubKeyName = subRegistryKey;
        }

        public override IReadOnlyCollection<Type> SupportedSettingDataTypes { get; } = new ReadOnlyCollection<Type>(new[]
        {
            typeof(string),
            typeof(int),
            typeof(byte[]),
        });

        public override object Select(CompoundSettingKey keys)
        {
            var registryPath = new RegistryPath(keys.NameKey.Value);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SubKeyName);
            using (var subKey = _baseRegistryKey.OpenSubKey(subKeyName, false))
            {
                var value = subKey?.GetValue(registryPath.ValueName);
                return value;
            }
        }

        public override void Update(CompoundSettingKey keys, object value)
        {
            var registryPath = new RegistryPath(keys.NameKey.Value);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SubKeyName);

            using (var subKey =
                _baseRegistryKey.OpenSubKey(subKeyName, true)
                ?? _baseRegistryKey.CreateSubKey(subKeyName)
            )
            {
                RegistryValueKind registryValueKind;
                if (!_supportedRegistryValueKinds.TryGetValue(value.GetType(), out registryValueKind))
                {
                    throw new UnsupportedRegistryTypeException
                    {
                        ValueTypeName = value.GetType().Name,
                        SettingName = keys.NameKey.Value.ToString()
                    };
                }
                subKey.SetValue(registryPath.ValueName, value, registryValueKind);
            }
        }
    }
}
