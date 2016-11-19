using System;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryPath : SettingUrn
    {
        public static readonly string DefaultDelimiter = @"\";

        public RegistryPath(SettingUrn urn) : base(urn)
        {
            Delimiter = DefaultDelimiter;
        }

        public static RegistryPath Parse(string value)
        {
            var settingPath = SettingUrn.Parse(value, DefaultDelimiter);
            return new RegistryPath(settingPath);
        }

        public bool IsLike(RegistryPath path)
        {
            return Name.Equals(path.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
