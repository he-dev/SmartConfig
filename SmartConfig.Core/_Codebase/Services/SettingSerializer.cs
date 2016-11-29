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
    internal class SettingSerializer
    {
        public SettingSerializer(TypeConverter converter)
        {
            Converter = converter;
        }

        public TypeConverter Converter { get; }     

        public Dictionary<SettingProperty, IList<Setting>> SerializeSettings(IEnumerable<SettingProperty> settings, IEnumerable<Type> supportedTypes)
        {
            var results = new Dictionary<SettingProperty, IList<Setting>>();

            var serializationExceptions = new List<Exception>();

            foreach (var setting in settings)
            {
                try
                {
                    results[setting] = SerializeSetting(setting, supportedTypes);
                }
                catch (Exception ex)
                {
                    serializationExceptions.Add(ex);
                }
            }

            if (serializationExceptions.Any())
            {
                throw new AggregateException(
                    $"Unable to serialize {serializationExceptions.Count} setting{(serializationExceptions.Count == 1 ? string.Empty : "s")}.",
                    serializationExceptions);
            }

            return results;
        }

        private IList<Setting> SerializeSetting(SettingProperty setting, IEnumerable<Type> supportedTypes)
        {
            var convert = new Func<object, Type, object>((obj, type) =>
            {
                if (obj.GetType() == type)
                {
                    return obj;
                }
                var result = Converter.Convert(obj, type, CultureInfo.InvariantCulture);
                return result;
            });

            if (setting.IsItemized)
            {
                var items = CollectionItemizer.ItemizeCollection(setting.Value);
                var settingType =
                    supportedTypes.FirstOrDefault(t => t == setting.Type.GetElementType()) ??
                    supportedTypes.FirstOrDefault(t => t == typeof(string));

                // todo: create exception
                if (settingType == null) { throw new Exception("Usupported type."); }

                var settings = items.Select(x => new Setting
                {
                    Name = new SettingUrn(setting.Path, (string)convert(x.Key, typeof(string))),
                    Value = convert(x.Value, settingType)
                })
                .ToList();

                return settings;
            }
            else
            {
                var settingType =
                    supportedTypes.FirstOrDefault(t => t == setting.Type) ??
                    supportedTypes.FirstOrDefault(t => t == typeof(string));

                var value = convert(setting.Value, settingType);
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
    }
}
