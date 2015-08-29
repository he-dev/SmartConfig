using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a non optional setting wasn't found in the source.
    /// </summary>
    public class OptionalException : SmartConfigException
    {
        internal OptionalException(SettingInfo settingInfo) : base(settingInfo, null) { }

        public override string Message => "This setting is not optional ConfigType = \"$ConfigTypeName\" SettingPath = \"$SettingPath\". If you want it to be optional add the $OptionalAttributeName."
            .FormatWith(new
            {
                ConfigTypeName = SettingInfo.ConfigType.Name,
                SettingPath = SettingInfo.SettingPath.ToString(),
                OptionalAttributeName = typeof(OptionalAttribute).Name
            }, true);
    }
}
