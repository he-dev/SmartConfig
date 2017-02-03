using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Reusable;
using SmartConfig.Data;
using Reusable.Converters;

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
                    var data = GetData(settingProperty, settings);
                    if (data == null && settingProperty.IsOptional) continue;
                    var value = _converter.Convert(data, settingProperty.Type);

                    var validationErrorMessages = settingProperty.Validations.Where(x => !x.IsValid(value)).Select(x => x.ErrorMessage).ToList();
                    if (validationErrorMessages.Any())
                    {
                        throw new Exception(string.Join(", ", validationErrorMessages));
                    }

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

            CommitSettingValues(cache);
        }

        private static object GetData(SettingProperty settingProperty, ICollection<Setting> values)
        {
            if (IsItemized(values))
            {
                if (settingProperty.Type.IsDictionary())
                {
                    return values.ToDictionary(x => x.Name.Key, x => x.Value);
                }

                if (settingProperty.Type.IsEnumerable())
                {
                    return values.Select(x => x.Value);
                }

                // TODO unknown setting type
                throw new Exception();
            }
            else
            {
                return values.SingleOrDefault()?.Value;
            }
        }

        private static bool IsItemized(ICollection<Setting> values)
        {
            // Settings is itemized if all values have a key.
            var keyCount = values.Count(x => x.Name.HasKey);

            // If key-count does not match the value-count then something is wrong.
            if (keyCount > 0 && keyCount != values.Count) throw new Exception("Invalid setting.");

            return keyCount > 0;
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
