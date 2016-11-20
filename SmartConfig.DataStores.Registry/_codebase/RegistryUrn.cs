using System;
using SmartConfig.Data;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryUrn : SettingUrn
    {
        public static readonly string DefaultDelimiter = @"\";

        public RegistryUrn(SettingUrn urn) : base(urn)
        {
            Delimiter = DefaultDelimiter;
        }

        public static RegistryUrn Parse(string value)
        {
            var settingUrn = SettingUrn.Parse(value, DefaultDelimiter);
            return new RegistryUrn(settingUrn);
        }

        public bool IsLike(RegistryUrn registryUrn)
        {
            return WeakName.Equals(registryUrn.WeakName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
