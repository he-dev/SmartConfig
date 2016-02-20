using System;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class UnsupportedTypeSettings
    {
        public static Uri UriSetting { get; set; }
    }
}
