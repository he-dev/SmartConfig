using System.Collections.Generic;
using SmartConfig.Data;
using SmartConfig.Example.Data;

namespace SmartConfig.Examples
{
    [SmartConfig]
    [SettingName("Examples")]
    static class ExampleDbConfig
    {
        [SmartConfigProperties]
        public static class Properties
        {
            public static IDataSource DataSource { get; } = new DbSource<Setting>(ExampleAppConfig.ConnectionStrings.ExampleDb, "ExampleConfigTable");

            public static IEnumerable<SettingKey> CustomKeys { get; } = new[]
            {
                new SettingKey(nameof(CustomSetting.Environment), "Demo"),
                new SettingKey(nameof(CustomSetting.Version), "4.5.0")
            };
        }

        [Optional]
        public static string Greeting { get; set; } = "Hello SmartConfig!";
    }
}
