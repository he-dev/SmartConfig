using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SmartConfig.DataAnnotations;
using SmartUtilities;
using SmartUtilities.DataAnnotations;

namespace SmartConfig
{
    public class ClassNotStaticException : Exception
    {
        internal ClassNotStaticException(Type type)
        {
            Type = type;
            Message = $"Type '{type.Name}' must be a static class.";
        }
        public override string Message { get; }
        public Type Type { get; }

        public override string ToString() => this.ToJson();
    }

    public class SettingNotFoundException : Exception
    {
        internal SettingNotFoundException(SettingInfo setting)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            ConfigType = setting.ConfigType;
            Message = $"Setting '{setting}' not found. If it is opitonal mark it with the '{nameof(OptionalAttribute)}' otherwise you need to provide a value for it.";
        }
        public override string Message { get; }
        public string SettingPath { get; set; }
        public Type ConfigType { get; set; }

        public override string ToString() => this.ToJson();
    }

    public class SingleSettingException : Exception
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
        internal DataReadException(SettingInfo setting, Type dataStoreType, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.SettingPath.FullNameEx;
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
        internal DeserializationException(SettingInfo setting, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            Message = $"Could not convert '{SettingPath}' to '{setting.Type.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }

        public override string ToString() => this.ToJson();
    }

    public class SerializationException : Exception
    {
        internal SerializationException(SettingInfo setting, Exception innerException) : base(null, innerException)
        {
            SettingPath = setting.SettingPath.FullNameEx;
            Message = $"Could not convert '{SettingPath}' to '{setting.Type.Name}'. See the inner exception for details about that.";
        }
        public override string Message { get; }
        public string SettingPath { get; }

        public override string ToString() => this.ToJson();
    }
}
