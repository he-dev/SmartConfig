using System;
using System.Reflection;
using Reusable.Data.DataAnnotations;
using Reusable.Extensions;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;

namespace SmartConfig
{
    public class ConfigurationLoadException : Exception
    {
        internal ConfigurationLoadException(Type configType, Type dataSourceType, Exception innerException) : base(null, innerException)
        {
            ConfigType = configType;
            DataSourceType = dataSourceType;
        }

        public Type ConfigType { get; set; }

        public Type DataSourceType { get; set; }

        public override string Message => $"Could not load \"{ConfigType.Name}\" from {DataSourceType.Name}.";
    }

    public class ConfigurationSaveException : Exception
    {
        internal ConfigurationSaveException(Type configType, Type dataSourceType, Exception innerException) : base(null, innerException)
        {
            ConfigType = configType;
            DataSourceType = dataSourceType;
        }

        public Type ConfigType { get; set; }

        public Type DataSourceType { get; set; }

        public override string Message => $"Could not save \"{ConfigType.Name}\" into {DataSourceType.Name}.";
    }

    public class SettingNotFoundException : Exception
    {
        internal SettingNotFoundException(string weakFullName)
        {
            WeakFullName = weakFullName;
        }

        public string WeakFullName { get; }

        public override string Message => $"Setting \"{WeakFullName}\" not found. You need to provide a value for it or decorate it with \"{nameof(OptionalAttribute)}\".";

        public override string ToString() => this.ToJson();
    }


    public class MultipleSettingsFoundException : Exception
    {
        internal MultipleSettingsFoundException(string weakFullName, int count)
        {
            WeakFullName = weakFullName;
            Count = count;
        }

        public string WeakFullName { get; }
        public int Count { get; }
        public override string Message => $"Setting \"{WeakFullName}\" found {Count} times. You need to remove the other settings or decorate it with the \"{nameof(ItemizedAttribute)}\".";

        public override string ToString() => this.ToJson();
    }

    public class SmartConfigAttributeNotFoundException : Exception
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

        public override string ToString() => this.ToJson();
    }

    public class DataReadException : Exception
    {
        internal DataReadException(SettingProperty setting, Type dataStoreType, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.Path.StrongFullName;
            DataStoreType = dataStoreType;
            Message = $"Could not read '{SettingPath}' from '{dataStoreType.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }
        public Type DataStoreType { get; }

        public override string ToString() => this.ToJson();
    }

    public class DataWriteException : Exception
    {
        public DataWriteException(Type dataStoreType, Exception innerException) : base(null, innerException)
        {
            DataStore = dataStoreType;
            Message = $"Could not save settings to '{DataStore.FullName}'.";
        }
        public override string Message { get; }
        public Type DataStore { get; }

        public override string ToString() => this.ToJson();
    }

    public class DeserializationException : Exception
    {
        internal DeserializationException(SettingProperty setting, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.Path.StrongFullName;
            Message = $"Could not convert '{SettingPath}' to '{setting.Type.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }

        public override string ToString() => this.ToJson();
    }

    public class SerializationException : Exception
    {
        internal SerializationException(SettingProperty setting, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.Path.StrongFullName;
            Message = $"Could not convert '{SettingPath}' to '{setting.Type.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }

        public override string ToString() => this.ToJson();
    }
}
