using System;

namespace SmartConfig
{
    public class ConfigurationSaveException : Exception
    {
        internal ConfigurationSaveException(Type configType, Type dataSourceType, Exception innerException) : base(null, innerException)
        {
            ConfigType = configType;
            DataSourceType = dataSourceType;
        }

        public Type ConfigType { get; set; }

        public Type DataSourceType { get; set; }

        public override string Message => $"Could not save \"{ConfigType.Name}\" to {DataSourceType.Name}.";
    }
}