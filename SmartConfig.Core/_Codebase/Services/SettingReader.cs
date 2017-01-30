using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using Reusable.Converters;

namespace SmartConfig.Services
{
    internal class SettingReader
    {
        private readonly DataStore _dataStore;
        private readonly IEnumerable<SettingProperty> _settingProperties;
        private readonly IDictionary<string, object> _tags;
        private readonly TypeConverter _converter;

        public SettingReader(
            DataStore dataStore,
            IEnumerable<SettingProperty> settingProperties,
            IDictionary<string, object> tags,
            TypeConverter converter
        )
        {
            _dataStore = dataStore;
            _settingProperties = settingProperties;
            _tags = tags;
            _converter = converter;
        }

        public void Read()
        {
            var cache = new Dictionary<SettingProperty, object>();

            // Deserialization exceptions are collected and rethrown aggregated.
            var exceptions = new List<Exception>();

            foreach (var settingProperty in _settingProperties)
            {
                try
                {
                    var settings = _dataStore.GetSettings(new Setting { Name = settingProperty.Path, Tags = _tags }).ToList();

                    if (!settings.Any())
                    {
                        if (!settingProperty.IsOptional) { throw new SettingNotFoundException(settingProperty.Path.WeakFullName); }
                        // Don't do anything. Just ignore optional settings.
                    }
                    else
                    {
                        ValidateValueCount(settingProperty, settings);
                        var data = GetData(settingProperty, settings);
                        cache[settingProperty] = _converter.Convert(data, settingProperty.Type);
                    }
                }
                catch (Exception inner)
                {
                    exceptions.Add(inner);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    message: $"Unable to deserialize {exceptions.Count} setting{(exceptions.Count == 1 ? string.Empty : "s")}.",
                    innerExceptions: exceptions);
            }

            CommitSettingValues(cache);
        }

        private static void ValidateValueCount(SettingProperty setting, ICollection<Setting> values)
        {
            if (!setting.IsEnumerable && values.Count > 1)
            {
                throw new MultipleSettingsFoundException(setting.Path.WeakFullName, values.Count);
            }
        }
        
        private static object GetData(SettingProperty settingProperty, IEnumerable<Setting> values)
        {
            if (settingProperty.IsItemized)
            {
                if (settingProperty.Type.IsDictionary())
                {
                    return values.ToDictionary(x => x.Name.Key, x => x.Value);
                }

                if (settingProperty.Type.IsEnumerable())
                {
                    return values.Select(x => x.Value);
                }
            }

            return values.Single().Value;
        }

        private static void CommitSettingValues(Dictionary<SettingProperty, object> settingValues)
        {
            foreach (var item in settingValues)
            {
                var settingProperty = item.Key;
                settingProperty.Value = item.Value;
            }
        }
    }
}
