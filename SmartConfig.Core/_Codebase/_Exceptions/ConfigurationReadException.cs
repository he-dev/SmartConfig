using System;

namespace SmartConfig
{
    public class ConfigurationReadException : Exception
    {
        internal ConfigurationReadException(Type dataSourceType, Exception innerException) : base(null, innerException)
        {
            DataSourceType = dataSourceType;
        }

        public Type DataSourceType { get; set; }

        public override string Message => $"Could not read from \"{DataSourceType.Name}\".";
    }
}