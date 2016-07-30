using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SmartConfig.DataAnnotations;
using SmartUtilities;
using SmartUtilities.DataAnnotations;

namespace SmartConfig
{
    // occurs when setting update did not work
    public class SaveSettingsException : FormattableException
    {
        public SaveSettingsException(Exception innerException) : base(innerException) { }
        public override string Message => $"Could not save settings to '{DataStore.FullName}'.";
        public Type DataStore { get; internal set; }
    }

    public class ClassNotStaticException : FormattableException
    {
        internal ClassNotStaticException(Type type) : base()
        {
            Type = type;
            Message = $"Type '{type.FullName}' must be a static class.";
        }
        public override string Message { get; }
        public Type Type { get; }
    }

    public class SettingNotFoundException : FormattableException
    {
        internal SettingNotFoundException(SettingInfo setting)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            ConfigType = setting.ConfigType;
            Message = $"Setting '{SettingPath}' not found. If it is opitonal mark it with the '{nameof(OptionalAttribute)}' otherwise you need to provide a value for it.";
        }
        public override string Message { get; }
        public string SettingPath { get; set; }
        public Type ConfigType { get; set; }
    }

    public class SingleSettingException:FormattableException
    {
        internal SingleSettingException(SettingInfo setting)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            ConfigType = setting.ConfigType;
            Message = $"Setting '{SettingPath}' found more then once but its type '{setting.Type.Name}' is not a collection.";
        }
        public override string Message { get; }
        public string SettingPath { get; set; }
        public Type ConfigType { get; set; }
    }

    public class SmartConfigAttributeNotFoundException : FormattableException
    {
        internal SmartConfigAttributeNotFoundException(Type type)
        {
            ConfigType = type;
            Message = $"Type '{type.FullName}' must be decorated with the '{nameof(SmartConfigAttribute)}'";
        }

        internal SmartConfigAttributeNotFoundException(PropertyInfo property)
        {
            Message = $"Property '{property.Name}' must be inside a static type that is decorated with the '{nameof(SmartConfigAttribute)}'";
        }

        public override string Message { get; }
        public Type ConfigType { get; }
    }

    public class DataReadException : FormattableException
    {
        internal DataReadException(SettingInfo setting, Type dataStoreType, Exception innerException) : base(innerException)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            DataStoreType = dataStoreType;
            Message = $"Could not read '{SettingPath}' from '{dataStoreType.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }
        public Type DataStoreType { get; }
    }

    public class ReadSettingException : FormattableException
    {
        internal ReadSettingException(SettingInfo setting, Exception innerException) : base(innerException)
        {
            Message = $"Could not read '{setting.SettingPath.FullNameEx}'.";
        }
        public override string Message { get; }
        public Type DataStoreType { get; internal set; }
        public Type ConfigurationType { get; internal set; }
        public string SettingPath { get; internal set; }
    }

    public class InvalidPropertyNameException : FormattableException
    {
        public string PropertyName { get; internal set; }
        public string SettingType { get; internal set; }
    }

    public class FilterAttributeMissingException : FormattableException
    {
        public string SettingType { get; internal set; }
        public string Property { get; internal set; }
    }

    public class CustomKeyNullException : FormattableException
    {
        public override string Message => $"Custom keys '{string.Join(", ", NullKeys)}' must not be null.";
        public string SettingType { get; internal set; }
        public IList<string> NullKeys { get; internal set; }
    }
}
