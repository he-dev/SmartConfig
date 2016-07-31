using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.TypeConversion;
using SmartUtilities.TypeFramework;
// ReSharper disable RedundantIfElseBlock

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
                    settingsCache.AddRange(SerializeSetting(setting, namespaces));
                }
                catch (Exception ex) { saveExceptions.Add(ex); }
            }

            if (saveExceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to serialize {saveExceptions.Count} setting{(saveExceptions.Count == 1 ? string.Empty : "s")}.",
                    saveExceptions);
            }

            return Commit(settingsCache);
        }

        private IEnumerable<Setting> SerializeSetting(SettingInfo setting, IReadOnlyDictionary<string, object> namespaces)
        {
            try
            {
                if (setting.IsItemized)
                {
                    var rows =
                        CollectionItemizer.ItemizeCollection(setting.Value, DataStore.MapDataType, Converter)
                        .Select(x => new Setting(new SettingPath(setting.SettingPath, x.Key), namespaces, x.Value));
                    return rows;
                }
                else
                {
                    var dataType = DataStore.MapDataType(setting.Type);
                    var value = dataType == setting.Type ? setting.Value : Converter.Convert(setting.Value, dataType);
                    var row = new Setting(setting.SettingPath, namespaces, value);
                    return new[] { row };
                }
            }
            catch (Exception innerException)
            {
                throw new SerializationException(setting, innerException);
            }

        }

        private int Commit(IReadOnlyCollection<Setting> settings)
        {
            try
            {
                var affectedSettings = DataStore.SaveSettings(settings);
                return affectedSettings;
            }
            catch (Exception innerException)
            {
                throw new DataWriteException(DataStore.GetType(), innerException);
            }
        }
    }
}
