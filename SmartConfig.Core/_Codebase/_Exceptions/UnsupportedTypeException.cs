using System;
using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a type was used that is not supported by an object converter.
    /// </summary>
    public class UnsupportedTypeException : Exception
    {
        internal UnsupportedTypeException(Type converterType, Type settingType)
        {
            ConverterType = converterType;
            SettingType = settingType;
        }

        public override string Message =>
            $"Converter = \"{ConverterType.Name}\" does not support " +
            $"SettingType = \"{SettingType.Name}\"";

        public Type ConverterType { get; private set; }

        public Type SettingType { get; private set; }
    }
}
