using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using Reusable.Converters;

// ReSharper disable RedundantIfElseBlock

namespace SmartConfig.Services
{
    internal class SettingWriter
    {
        private readonly DataStore _dataStore;
        private readonly IEnumerable<SettingProperty> _settingProperties;
        private readonly IDictionary<string, object> _tags;
        private readonly TypeConverter _converter;
        private readonly TypeConverter _itemizer;


        public SettingWriter(
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
            if (setting.IsItemized)
            {
                var settingType = GetSettingType(setting.Type.GetElementType());
                var items = (IDictionary<object, object>)_itemizer.Convert(setting.Value, typeof(Dictionary<object, object>));
                var settings = items.Select(x => new Setting
                {
                    Name = new SettingUrn(setting.Path, (string)_converter.Convert(x.Key, typeof(string))),
                    Value = _converter.Convert(x.Value, settingType)
                })
                .ToList();

                return settings;
            }
            else
            {
                var settingType = GetSettingType(setting.Type);
                var value = _converter.Convert(setting.Value, settingType);
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

        private Type GetSettingType(Type type)
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
                _dataStore.SaveSettings(settings);
            }
        }
    }
}
