using System;

namespace SmartConfig.Core.Tests.TestConfigs
{
    [SmartConfig]
    public static class UnsupportedTypeSettings
    {
        public static Uri UriSetting { get; set; }
    }
}
