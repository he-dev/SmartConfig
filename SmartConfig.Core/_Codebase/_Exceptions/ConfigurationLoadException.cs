using System;

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

        public override string Message => $"Could not load \"{ConfigType.Name}\" from \"{DataSourceType.Name}\".";
    }
}