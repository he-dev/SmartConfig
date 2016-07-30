using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.TypeFramework;

namespace SmartConfig.IO
{
    internal class SettingWriter
    {
        public SettingWriter(IDataStore dataStore, TypeConverter converter)
        {
            DataStore = dataStore;
            Converter = converter;
        }

        public IDataStore DataStore { get; }

        public TypeConverter Converter { get; }

        public int SaveSettings(IReadOnlyCollection<SettingInfo> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            var settingsCache = new List<Setting>();
            var saveExceptions = new List<Exception>();

            foreach (var setting in settings)
            {
                try
                {
                    if (setting.IsItemized)
                    {
                        var items =
                            CollectionItemizer.ItemizeCollection(setting.Value, DataStore.MapDataType, Converter)
                            .Select(x => new Setting(new SettingPath(setting.SettingPath, x.Key), namespaces, x.Value));
                        settingsCache.AddRange(items);
                    }
                    else
                    {
                        var dataType = DataStore.MapDataType(setting.Type);
                        var value = dataType == setting.Type
                            ? setting.Value
                            : Converter.Convert(setting.Value, dataType, CultureInfo.InvariantCulture);
                        settingsCache.Add(new Setting(setting.SettingPath, namespaces, value));
                    }
                }
                catch (Exception ex) { saveExceptions.Add(ex); }
            }

            if (saveExceptions.Any())
            {
                throw new AggregateException("Unable to prepare one or more settings for saving.", saveExceptions);
            }

            return Commit(settingsCache);
        }

        private int Commit(IReadOnlyCollection<Setting> settings)
        {
            try
            {
                var affectedSettings = DataStore.SaveSettings(settings);
                return affectedSettings;
            }
            catch (Exception ex)
            {
                throw new SaveSettingsException(ex) { DataStore = DataStore.GetType() };
            }
        }
    }
}
