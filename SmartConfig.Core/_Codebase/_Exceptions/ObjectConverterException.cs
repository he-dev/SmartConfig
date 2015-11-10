using System;
using SmartConfig.Reflection;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not convert a value. See the inner exception for more details.
    /// </summary>
    public class ObjectConverterException : SmartConfigException
    {
        public ObjectConverterException(object value, SettingInfo settingInfo, Exception innerException)
            : base(settingInfo, innerException)
        {
            Value = value;
        }

        public override string Message => 
            $"Error converting " +
            $"SettingPath = \"{SettingInfo.SettingPath}\" " +
            $"SettingType = \"{SettingInfo.SettingType.Name}\". " +
            $"See inner exeption for details.";

        public object Value { get; private set; }
    }
}
