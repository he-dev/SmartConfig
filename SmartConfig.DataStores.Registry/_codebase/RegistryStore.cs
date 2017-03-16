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
using SmartConfig.Collections;
using SmartConfig.Data;

// ReSharper does not understad C# 7
// ReSharper disable HeuristicUnreachableCode

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
            _baseKey = baseKey ?? throw new ArgumentNullException(nameof(baseKey));
            _baseSubKeyName = subKey.NonEmptyOrNull() ?? throw new ArgumentNullException(nameof(subKey));
        }

        public override IEnumerable<Setting> ReadSettings(Setting setting)
        {
            var registryPath = new RegistryPath(setting.Name);

            var subKeyName = Path.Combine(_baseSubKeyName, registryPath.Namespace);
            using (
                var subKey = _baseKey.OpenSubKey(subKeyName, false) ??
                throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName)
            )
            {
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
            void DeleteObsoleteSettings(RegistryKey registryKey, IGrouping<Setting, Setting> group)
            {
                var obsoleteNames =
                    registryKey
                        .GetValueNames()
                        .Where(x => RegistryPath.Parse(x).IsLike(group.Key.Name))
                        .ToList();

                obsoleteNames.ForEach(registryKey.DeleteValue);
            }

            foreach (var group in settings)
            {
                var subKeyName = Path.Combine(_baseSubKeyName, group.Key.Name.Namespace);
                using (var subKey =
                    _baseKey.OpenSubKey(subKeyName, true) ??
                    _baseKey.CreateSubKey(subKeyName) ??
                    throw new OpenOrCreateSubKeyException(_baseKey.Name, _baseSubKeyName, subKeyName)
                )
                {
                    DeleteObsoleteSettings(subKey, group);

                    foreach (var setting in group)
                    {
                        if (!_registryValueKinds.TryGetValue(setting.Value.GetType(), out RegistryValueKind registryValueKind))
                        {
                            throw new InvalidTypeException(setting.Value.GetType(), SupportedTypes);
                        }

                        var registryUrn = new RegistryPath(setting.Name);

                        subKey.SetValue(registryUrn.StrongName, setting.Value, registryValueKind);
                    }
                }
            }
        }

        public const string DefaultKey = @"Software\SmartConfig";

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
