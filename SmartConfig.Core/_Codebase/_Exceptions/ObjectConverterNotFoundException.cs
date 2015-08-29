using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when an object converter could not be found.
    /// </summary>
    public class ObjectConverterNotFoundException : SmartConfigException
    {
        internal ObjectConverterNotFoundException(SettingInfo settingInfo) : base(settingInfo, null)
        {
        }

        public override string Message => "Object converter not found. ConverterType = \"$ConverterTypeName\" ConfigType = \"$ConfigTypeName\" SettingPath = \"$SettingPath\""
            .FormatWith(new
            {
                ConverterTypeName = SettingInfo.ConverterType.Name,
                ConfigTypeName = SettingInfo.ConfigType.Name,
                SettingPath = SettingInfo.SettingPath.ToString()
            }, true);
    }
}
