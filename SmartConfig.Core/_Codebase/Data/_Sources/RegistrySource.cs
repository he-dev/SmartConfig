using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SmartConfig.Collections;

namespace SmartConfig.Data
{
    public class RegistrySource<TSetting> : DataSource<TSetting> where TSetting : Setting, new()
    {
        private readonly RegistryKey _baseRegistryKey;
        private readonly string _baseSubKeyName;
        private readonly IDictionary<Type, RegistryValueKind> _supportedRegistryValueKinds = new Dictionary<Type, RegistryValueKind>
        {
            { typeof(string), RegistryValueKind.String },
            { typeof(int), RegistryValueKind.DWord },
            { typeof(byte[]), RegistryValueKind.Binary },
        };


        public RegistrySource(RegistryKey baseRegistryKey, string baseSubKeyName = null)
        {
            if (baseRegistryKey == null) { throw new ArgumentNullException(nameof(baseRegistryKey)); }

            _baseRegistryKey = baseRegistryKey;
            _baseSubKeyName = string.IsNullOrEmpty(baseSubKeyName) ? string.Empty : baseSubKeyName;
        }

        public override IReadOnlyCollection<Type> SupportedTypes { get; } = new ReadOnlyCollection<Type>(new[]
        {
            typeof(string),
            typeof(int),
            typeof(byte[]),
        });

        public override object Select(SettingKeyReadOnlyCollection keys)
        {
            var registryPath = new RegistryPath((SettingPath)keys.DefaultKey.Value);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SubKeyName);
            using (var subKey = _baseRegistryKey.OpenSubKey(subKeyName, false))
            {
                var value = subKey?.GetValue(registryPath.ValueName);
                return value;
            }
        }

        public override void Update(SettingKeyReadOnlyCollection keys, object value)
        {
            var registryPath = new RegistryPath((SettingPath)keys.DefaultKey.Value);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.SubKeyName);
            using (var subKey = _baseRegistryKey.OpenSubKey(subKeyName, true) ?? _baseRegistryKey.CreateSubKey(subKeyName))
            {
                RegistryValueKind registryValueKind;
                if (!_supportedRegistryValueKinds.TryGetValue(value.GetType(), out registryValueKind))
                {
                    throw new UnsupportedRegistryTypeException
                    {
                        ValueTypeName = value.GetType().Name,
                        SettingName = keys.DefaultKey.Value.ToString()
                    };
                }
                subKey.SetValue(registryPath.ValueName, value, registryValueKind);
            }
        }        
    }
}
