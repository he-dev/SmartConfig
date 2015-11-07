using System.Drawing;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class ColorSettings
    {
        public static Color NameColorSetting { get; set; }
        public static Color DecColorSetting { get; set; }
        public static Color HexColorSetting { get; set; }
    }
}
