using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            var settingPath = SettingPath.Parse(value, DefaultDelimiter);
            return new RegistryPath(settingPath);
        }
    }
}
