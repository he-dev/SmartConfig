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
        private readonly List<Setting> _cache = new List<Setting>();

        public SettingWriter(IDataStore dataStore, TypeConverter converter)
        {
            DataStore = dataStore;
            Converter = converter;
        }

        public IDataStore DataStore { get; }

        public TypeConverter Converter { get; }

        public int SaveSettings(IReadOnlyCollection<SettingInfo> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            _cache.Clear();
            var validationExceptions = new List<Exception>();

            foreach (var setting in settings)
            {
                try
                {
                    if (setting.IsItemized)
                    {
                        var items = CollectionItemizer.ItemizeCollection(setting.Value, DataStore.MapDataType, Converter);
                        foreach (var item in items)
                        {
                            var s = new Setting(new SettingPath(setting.SettingPath, item.Key), namespaces, item.Value);
                            _cache.Add(s);
                        }
                    }
                    else
                    {
                        var dataType = DataStore.MapDataType(setting.Type);
                        var value = dataType == setting.Type
                            ? setting.Value
                            : Converter.Convert(setting.Value, dataType, CultureInfo.InvariantCulture);
                        _cache.Add(new Setting(setting.SettingPath, namespaces, value));
                    }
                }
                catch (Exception ex) { validationExceptions.Add(ex); }
            }
            return Commit();
        }

        private int Commit()
        {
            try
            {
                var affectedSettings = DataStore.SaveSettings(_cache);
                return affectedSettings;
            }
            catch (Exception ex)
            {
                throw new SaveSettingsException(ex) { DataStore = DataStore.GetType() };
            }
            finally
            {
                _cache.Clear();
            }
        }
    }
}
