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

        //List<Setting> GetSettings(SettingPath path, IReadOnlyDictionary<string, object> namespaces);
        List<Setting> GetSettings(Setting setting);

        //int SaveSetting(SettingPath path, IReadOnlyDictionary<string, object> namespaces, object value);
        int SaveSetting(Setting setting);

        //int SaveSettings(IReadOnlyDictionary<SettingPath, object> settings, IReadOnlyDictionary<string, object> namespaces);
        int SaveSettings(IReadOnlyCollection<Setting> settings);
    }
}
