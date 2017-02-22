using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SmartConfig.Data;
using SmartConfig.Collections;
using SmartConfig.Converters;
using Reusable.TypeConversion;

// ReSharper disable RedundantIfElseBlock

namespace SmartConfig.Services
{
    // Writes settings from setting-properties into data-store.
    internal class SettingWriter
    {
        private readonly DataStore _dataStore;
        private readonly IEnumerable<SettingProperty> _settingProperties;
        private readonly TagCollection _tags;
        private readonly TypeConverter _converter;
        private readonly Action<string> _log;
        private readonly TypeConverter _itemizer;

        public SettingWriter(DataStore dataStore, IEnumerable<SettingProperty> settingProperties, TagCollection tags, TypeConverter converter, Action<string> log)
        {
            _dataStore = dataStore;
            _settingProperties = settingProperties;
            _tags = tags;
            _converter = converter;
            _log = log;
            _itemizer = TypeConverter.Empty.Add<EnumerableToDictionaryConverter>();
        }

        public void Write()
        {
            var cache = new Dictionary<SettingProperty, IList<Setting>>();

            var exceptions = new List<Exception>();

            foreach (var settingProperty in _settingProperties)
            {
                try
                {
                    cache[settingProperty] = SerializeSetting(settingProperty);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to serialize {exceptions.Count} setting{(exceptions.Count == 1 ? string.Empty : "s")}.",
                    exceptions);
            }

            Save(cache);
        }

        private IList<Setting> SerializeSetting(SettingProperty setting)
        {
            //var itemize = setting.Type.IsEnumerable() && !_converter.CanConvert(setting.Value, setting.Type);
            if (setting.IsItemized)
            {
                var storeType = GetStoreType(setting.Type.GetElementType());
                var items = (IDictionary)_itemizer.Convert(setting.Value, typeof(Dictionary<object, object>), setting.FormatString, setting.FormatProvider);
                var settings = items.Keys.Cast<object>().Select(key => new Setting
                {
                    Name = new SettingPath(setting.Path)
                    {
                        Key = (string)_converter.Convert(key, typeof(string))
                    },
                    Value = _converter.Convert(items[key], storeType)
                })
                .ToList();

                return settings;
            }
            else
            {
                var settingType = GetStoreType(setting.Type);
                var value = _converter.Convert(setting.Value, settingType, setting.FormatString, setting.FormatProvider ?? CultureInfo.InvariantCulture);
                var settings = new[]
                {
                    new Setting
                    {
                        Name = setting.Path,
                        Value = value
                    }
                };
                return settings;
            }
        }

        private Type GetStoreType(Type type)
        {
            if (_dataStore.SupportedTypes.Any(supportedType => supportedType == type))
            {
                return type;
            }

            if (_dataStore.SupportedTypes.Any(supportedType => supportedType == typeof(string)))
            {
                return typeof(string);
            }

            throw new NotSupportedException($"'{type}' is not a supported data type.");
        }

        private void Save(IDictionary<SettingProperty, IList<Setting>> settingValues)
        {
            foreach (var item in settingValues)
            {
                var settings = item.Value;
                foreach (var setting in settings)
                {
                    setting.Tags = _tags;
                }
                _dataStore.WriteSettings(settings);
            }
        }
    }
}
