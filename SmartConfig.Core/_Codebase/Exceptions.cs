using System;
using System.Reflection;
using Reusable.Data.DataAnnotations;
using Reusable.Extensions;
using SmartConfig.Data;
using SmartConfig.DataAnnotations;

namespace SmartConfig
{
    public class SettingNotFoundException : Exception
    {
        internal SettingNotFoundException(SettingProperty setting)
        {
            SettingPath = setting.Path.FullNameWithKey;
            ConfigType = setting.ConfigType;
            Message = $"Setting '{setting}' not found. If it is opitonal mark it with the '{nameof(OptionalAttribute)}' otherwise you need to provide a value for it.";
        }
        public override string Message { get; }
        public string SettingPath { get; set; }
        public Type ConfigType { get; set; }

        public override string ToString() => this.ToJson();
    }

    public class MultipleSettingException : Exception
    {
        public override string Message => "Setting found more then once but it is not a collection.";

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
            SettingPath = setting.Path.FullNameWithKey;
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
            SettingPath = setting.Path.FullNameWithKey;
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
            SettingPath = setting.Path.FullNameWithKey;
            Message = $"Could not convert '{SettingPath}' to '{setting.Type.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }

        public override string ToString() => this.ToJson();
    }
}
