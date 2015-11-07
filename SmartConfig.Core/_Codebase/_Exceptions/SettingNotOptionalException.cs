using SmartUtilities;

namespace SmartConfig
{
    /// <summary>
    /// Occurs when a non optional setting wasn't found in the source.
    /// </summary>
    public class SettingNotOptionalException : SmartException
    {
        public string ConfigTypeFullName { get { return GetValue<string>(); } internal set { SetValue(value); } }
        public string SettingPath { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
