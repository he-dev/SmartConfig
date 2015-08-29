using System;

namespace SmartConfig
{
    internal static class SettingInfoFactory
    {
        public static SettingInfo CreateSettingsInitializedSettingInfo(Type configType)
        {
            return new SettingInfo(configType, KeyNames.Internal.SettingsInitializedKeyName, typeof(bool));
        }
    }
}
