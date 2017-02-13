using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Reusable;
using SmartConfig.Data;
using Reusable.Converters;
using SmartConfig.Collections;

namespace SmartConfig.Services
{
    //[AttributeUsage(AttributeTargets.All)]
    //public sealed class ItemRequiredAttribute : RequiredAttribute
    //{
    //    public override bool IsValid(object value)
    //    {
    //        var enumerable = value as IEnumerable;
    //        return enumerable?.Cast<object>().All(item => base.IsValid(item)) ?? false;
    //    }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        return IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage, validationContext. validationContext.MemberName);
    //    }
    //}

    // Reads settings from data-store into setting-properties.
    internal class SettingReader
    {
        private readonly DataStore _dataStore;
        private readonly IEnumerable<SettingProperty> _settingProperties;
        private readonly TagCollection _tags;
        private readonly TypeConverter _converter;
        private readonly Action<string> _log;

        public SettingReader(DataStore dataStore, IEnumerable<SettingProperty> settingProperties, TagCollection tags, TypeConverter converter, Action<string> log)
        {
            _dataStore = dataStore;
            _settingProperties = settingProperties;
            _tags = tags;
            _converter = converter;
            _log = log;
        }

        public void Read()
        {
            var cache = new Dictionary<SettingProperty, object>();

            // Deserialization exceptions are collected and rethrown aggregated.
            var exceptions = new List<Exception>();

            IList<Setting> ReadSettings(SettingProperty settingProperty) => _dataStore.ReadSettings(new Setting
            {
                Name = settingProperty.Path,
                Tags = _tags
            }).ToList();

            void ValidateSetting(SettingProperty settingProperty, object value)
            {
                foreach (var validation in settingProperty.Validations)
                {
                    validation.Validate(value, settingProperty.Path.WeakFullName);
                }
            }

            foreach (var settingProperty in _settingProperties)
            {
                try
                {
                    var settings = ReadSettings(settingProperty);
                    var data = GetSettingData(settingProperty, settings);
                    var value = data == null ? null : _converter.Convert(data, settingProperty.Type, settingProperty.FormatString, settingProperty.FormatProvider ?? CultureInfo.InvariantCulture);
                    ValidateSetting(settingProperty, value);
                    if (value == null) continue;
                    cache[settingProperty] = value;
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

            void CommitSettingValues(Dictionary<SettingProperty, object> settingValues)
            {
                foreach (var item in settingValues)
                {
                    var settingProperty = item.Key;
                    settingProperty.Value = item.Value;
                }
            }

            CommitSettingValues(cache);
        }

        private static object GetSettingData(SettingProperty settingProperty, ICollection<Setting> settings)
        {
            void ValidateItemizedSettings()
            {
                // All itemized settings must have keys.
                var keyCount = settings.Count(x => x.Name.HasKey);

                // If key-count does not match the setting-count then something is wrong.
                if (keyCount > 0 && keyCount != settings.Count) throw new ItemizedSettingException(settingProperty.Path);
            }

            if (settingProperty.IsItemized)
            {
                ValidateItemizedSettings();

                if (settingProperty.Type.IsDictionary())
                {
                    return settings.ToDictionary(x => x.Name.Key, x => x.Value);
                }

                if (settingProperty.Type.IsEnumerable())
                {
                    return settings.Select(x => x.Value);
                }

                throw new NotSupportedException($"Setting type '{settingProperty.Type}' is not supported for itemized settings.");
            }
            else
            {
                return settings.SingleOrDefault()?.Value;
            }
        }        
    }
}
