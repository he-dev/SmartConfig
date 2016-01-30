using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests.TestConfigs
{
    [SmartConfig]
    static class DatabaseSettings1
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static IDataSource DataSource { get; } =
                new DbSource<TestSetting>("name=TestDb", "TestSetting");

            public static IEnumerable<SettingKey> CustomKeys { get; } = new[]
            {
                new SettingKey("Environment", "A"),
                new SettingKey("Version", "2.2.1"),
            };
        }
        public static string StringSetting { get; set; }
        public static string Int32Setting { get; set; }
    }

    [SmartConfig]
    [SettingName("app2")]
    static class DatabaseSettings2
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static IDataSource DataSource { get; } =
                new DbSource<TestSetting>("name=TestDb", "TestSetting");
            public static IEnumerable<SettingKey> CustomKeys { get; } = new[]
            {
                new SettingKey("Environment", "A"),
                new SettingKey("Version", "5.0.4"),
            };
        }
        public static string StringSetting { get; set; }
        public static string Int32Setting { get; set; }
    }
}
