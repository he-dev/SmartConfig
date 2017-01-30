using System;

namespace SmartConfig
{
    public class ConfigurationWriteException : Exception
    {
        internal ConfigurationWriteException(Type dataSourceType, Exception innerException) : base(null, innerException)
        {
            DataSourceType = dataSourceType;
        }

        public Type DataSourceType { get; set; }

        public override string Message => $"Could not write to {DataSourceType.Name}.";
    }
}