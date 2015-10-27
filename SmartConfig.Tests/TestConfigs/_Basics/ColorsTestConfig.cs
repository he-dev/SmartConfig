using System.Drawing;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    public static class ColorsTestConfig
    {
        public static Color NameColorField { get; set; }
        public static Color DecColorField { get; set; }
        public static Color HexColorField { get; set; }
    }
}
