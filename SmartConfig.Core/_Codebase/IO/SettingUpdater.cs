using System;
using System.Diagnostics;
using System.Linq;
using SmartUtilities;
using SmartUtilities.Collections;
using SmartUtilities.ObjectConverters.DataAnnotations;

namespace SmartConfig.IO
{
    internal class SettingUpdater
    {
        public static void UpdateSetting(Setting setting)
        {
            Debug.Assert(setting != null);

            try
            {
                var dataSource = setting.Configuration.DataStore;
                var serializationType = dataSource.GetSerializationType(setting.NormalizedType);
                var converter = setting.Configuration.Converters[setting.NormalizedType];
                var serializedValue = converter.SerializeObject(setting.Value, serializationType, setting.ValidationAttributes);
                dataSource.Update(setting.Key, serializedValue);
            }
            catch (Exception inner)
            {
                throw SmartException.Create<UpdateSettingException>(ex =>
                {
                    ex.DataStore = setting.Configuration.DataStore.GetType().Name;
                    ex.Configuration = setting.Configuration.Type.Name;
                    ex.SettingPath = setting.Path;
                    ex.HelpText = "See inner exception for details.";
                }, inner);
            }
        }
    }
}
