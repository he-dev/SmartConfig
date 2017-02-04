using System;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryPath : SettingPath
    {
        public static readonly string DefaultDelimiter = @"\";

        public RegistryPath(SettingPath path) : base(path)
        {
            Delimiter = DefaultDelimiter;
        }

        public static RegistryPath Parse(string value)
        {
            var settingUrn = SettingPath.Parse(value, DefaultDelimiter);
            return new RegistryPath(settingUrn);
        }

        public bool IsLike(RegistryPath registryPath)
        {
            return WeakName.Equals(registryPath.WeakName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
