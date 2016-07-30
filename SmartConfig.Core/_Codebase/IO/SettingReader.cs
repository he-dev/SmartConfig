using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.TypeFramework;
using SmartUtilities.ValidationExtensions;

namespace SmartConfig.IO
{
    // This class knows how to read settings from the store and convert them to objects.
    internal class SettingReader
    {
        public SettingReader(IDataStore dataStore, TypeConverter converter)
        {
            DataStore = dataStore;
            Converter = converter;
        }

        public IDataStore DataStore { get; }

        public TypeConverter Converter { get; }

        public int ReadSettings(IReadOnlyCollection<SettingInfo> settings, IReadOnlyDictionary<string, object> namespaces)
        {
            var valuesCache = new Dictionary<SettingInfo, object>();
            var readExceptions = new List<Exception>();

            foreach (var setting in settings)
            {
                // Data read exceptions abort reading.
                var rows = ReadData(setting, namespaces);

                // Deserialization exceptions are collected and rethrown aggregated.
                try
                {
                    valuesCache[setting] = DeserializeSetting(setting, rows);
                }
                catch (Exception ex)
                {
                    readExceptions.Add(ex);
                }
            }

            if (readExceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to deserialize {readExceptions.Count} setting{(readExceptions.Count == 1 ? string.Empty : "s")}.", 
                    readExceptions);
            }

            return Commit(valuesCache);
        }

        private List<Setting> ReadData(SettingInfo setting, IReadOnlyDictionary<string, object> namespaces)
        {
            // This method wraps the original exception to provide additional information for the user.
            try
            {
                return DataStore.GetSettings(new Setting(setting.SettingPath, namespaces));
            }
            catch (Exception innerException)
            {
                throw new DataReadException(setting, DataStore.GetType(), innerException);
            }
        }

        private object DeserializeSetting(SettingInfo setting, IReadOnlyCollection<Setting> rows)
        {
            var culture = CultureInfo.InvariantCulture;

            if (!rows.Any() && !setting.IsOptional)
            {
                throw new SettingNotFoundException(setting);
            }

            if (!rows.Any() && setting.IsOptional)
            {
                return null;
            }

            if (setting.IsItemized)
            {
                var data = (object)null;

                if (setting.Type.IsArray)
                {
                    data = rows.Select(x => x.Value);
                }

                if (setting.Type.IsList())
                {
                    data = rows.Select(x => x.Value);
                }

                if (setting.Type.IsHashSet())
                {
                    data = rows.Select(x => x.Value);
                }

                if (setting.Type.IsDictionary())
                {
                    data = rows.ToDictionary(x => x.Name.ValueKey, x => x.Value);
                }

                var value = Converter.Convert(data, setting.Type, culture);
                return value;
            }
            else
            {
                if (rows.Count > 1)
                {
                    throw new SingleSettingException(setting);
                }

                var row0 = rows.Single();
                var dataHasTargetType = row0.Value.GetType() == setting.Type;
                var value = dataHasTargetType ? row0.Value : Converter.Convert(row0.Value, setting.Type, culture);
                return value;
            }
        }

        private int Commit(IReadOnlyDictionary<SettingInfo, object> values)
        {
            var affectedSettingCount = 0;
            
            // Optional settings might be null so we ignore them.
            foreach (var setting in values.Where(x => x.Value != null))
            {
                setting.Key.Value = setting.Value;
                affectedSettingCount++;
            }
            return affectedSettingCount;
        }
    }
}
