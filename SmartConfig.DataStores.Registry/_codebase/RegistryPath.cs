using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SmartConfig.DataStores.Registry
{
    public class RegistryPath : SettingPath
    {
        public RegistryPath(SettingPath path) : base(path)
        {
            Delimiter = @"\";
        }
    }
}
