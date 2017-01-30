using System;

namespace SmartConfig
{
    public class ConfigurationException : Exception
    {
        internal ConfigurationException(Type configType, Exception innerException) : base(null, innerException)
        {
            ConfigType = configType;
        }

        public Type ConfigType { get; set; }

        public override string Message => $"Could not read or write \"{ConfigType.Name}\".";
    }
}