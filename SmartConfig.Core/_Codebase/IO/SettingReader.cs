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
    // this class knows how to read settings
    internal class SettingReader
    {
        private readonly IDictionary<SettingInfo, object> _cache = new Dictionary<SettingInfo, object>();

        public SettingReader(IDataStore dataStore, TypeConverter converter)
        {
            DataStore = dataStore;
            Converter = converter;
        }

        public IDataStore DataStore { get; }

        public TypeConverter Converter { get; set; }

        public Dictionary<string, object> Namespaces { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public int ReadSettings(IEnumerable<SettingInfo> settings)
        {
            _cache.Clear();
            var validationExceptions = new List<Exception>();

            var culture = CultureInfo.InvariantCulture;

            foreach (var setting in settings)
            {
                try
                {
                    ReadSetting(setting);
                }
                catch (Exception ex)
                {
                    validationExceptions.Add(ex);
                }
            }

            if (validationExceptions.Any())
            {
                throw new AggregateException("Unable to read one or more settings.", validationExceptions);
            }

            return Commit();
        }

        private void ReadSetting(SettingInfo setting)
        {
            var culture = CultureInfo.InvariantCulture;

            // data store exeptions shouldn't be cought
            var settingRows = DataStore.GetSettings(setting.SettingPath, Namespaces);

            if (setting.IsItemized)
            {
                var data = (object)null;

                if (setting.Type.IsArray)
                {
                    data = settingRows.Select(x => x.Value);
                }

                if (setting.Type.IsList())
                {
                    data = settingRows.Select(x => x.Value);
                }

                if (setting.Type.IsHashSet())
                {
                    data = settingRows.Select(x => x.Value);
                }

                if (setting.Type.IsDictionary())
                {
                    data = settingRows.ToDictionary(x => x.Name.ValueKey, x => x.Value);
                }

                var value = Converter.Convert(data, setting.Type, culture);
                _cache[setting] = value;
            }
            else
            {
                settingRows.Count.Validate().IsTrue(x => x <= 1, ctx => $"'{setting.SettingPath.ToString()}' found more then once.");
                var settingRow = settingRows.SingleOrDefault();

                if (settingRow == null)
                {
                    if (setting.IsOptional)
                    {
                        return;
                    }
                    throw new SettingNotFoundException { SettingPath = setting.SettingPath };
                }

                var value =
                    settingRow.Value.GetType() == setting.Type
                        ? settingRow.Value
                        : Converter.Convert(settingRow.Value, setting.Type, culture);
                _cache[setting] = value;
            }
        }

        private int Commit()
        {
            var affectedSettingCount = 0;
            foreach (var setting in _cache.Where(x => x.Value != null))
            {
                setting.Key.Value = setting.Value;
                affectedSettingCount++;
            }
            _cache.Clear();
            return affectedSettingCount;
        }
    }
}
