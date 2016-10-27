using System;
using System.Collections.Generic;

namespace SmartConfig.Data
{
    /// <summary>
    /// Specifies the data source interface.
    /// </summary>
    public interface IDataStore
    {
        Type MapDataType(Type settingType);

        List<Setting> GetSettings(Setting setting);

        int SaveSetting(Setting setting);

        int SaveSettings(IReadOnlyCollection<Setting> settings);
    }
}
