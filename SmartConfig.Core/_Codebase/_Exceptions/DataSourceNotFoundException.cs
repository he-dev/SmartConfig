using System;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not be found.
    /// </summary>
    public class DataSourceNotFoundException : Exception
    {
        private readonly Type _configType;

        internal DataSourceNotFoundException(Type configType)
        {
            _configType = configType;
        }

        public override string Message => $"Data source not found. ConfigType = \"{_configType.Name}\"";
    }
}
