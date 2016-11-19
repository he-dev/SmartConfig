using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Reusable;
using Reusable.Extensions;
using SmartConfig.Data;

namespace SmartConfig.Services
{
    internal class SettingDeserializer
    {
        public SettingDeserializer(TypeConverter converter)
        {
            Converter = converter;
        }

        private TypeConverter Converter { get; }        

        public Dictionary<SettingProperty, object> DeserializeSettings(Dictionary<SettingProperty, List<Setting>> settings)
        {
            var settingValues = new Dictionary<SettingProperty, object>();

            // Deserialization exceptions are collected and rethrown aggregated.
            var deserializationExceptions = new List<Exception>();

            foreach (var item in settings)
            {
                var setting = item.Key;
                var values = item.Value;

                try
                {
                    if (!values.Any())
                    {
                        if (!setting.IsOptional)
                        {
                            throw new SettingNotFoundException(setting);
                        }
                        // Don't do anything. Just ignore optional settings.
                        continue;
                    }

                    settingValues[setting] = DeserializeSetting(values, setting.Type, setting.IsItemized);
                }
                catch (Exception ex)
                {
                    deserializationExceptions.Add(ex);
                }
            }

            if (deserializationExceptions.Any())
            {
                throw new AggregateException(
                    message: $"Unable to deserialize {deserializationExceptions.Count} setting{(deserializationExceptions.Count == 1 ? string.Empty : "s")}.",
                    innerExceptions: deserializationExceptions);
            }

            return settingValues;
        }

        private object DeserializeSetting(ICollection<Setting> values, Type settingType, bool isItemized)
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

            if (isItemized)
            {
                var data = (object)null;                

                if (settingType.IsArray)
                {
                    data = values.Select(x => x.Value);
                }

                if (settingType.IsList())
                {
                    data = values.Select(x => x.Value);
                }

                if (settingType.IsHashSet())
                {
                    data = values.Select(x => x.Value);
                }

                if (settingType.IsDictionary())
                {
                    data = values.ToDictionary(x => x.Name.Key, x => x.Value);
                }

                return convert(data, settingType);
            }
            else
            {
                if (values.Count > 1)
                {
                    throw new MultipleSettingsFoundException();
                }

                return convert(values.Single().Value, settingType);
            }
        }       
    }
}
