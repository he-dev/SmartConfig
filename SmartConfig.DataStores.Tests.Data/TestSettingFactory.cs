using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Reusable;
using SmartConfig.DataStores.Tests.Common;

namespace SmartConfig.DataStores.Tests.Data
{
    public class TestSettingFactory
    {
        public static IEnumerable<TestSetting> CreateTestSettings1()
        {
            return 
                from setting in ReadTestSettings()
                let tags = new Dictionary<string, string>()
                select new TestSetting
                {
                    Name = setting.Key,
                    Value = setting.Value,
                    Tags = tags
                };           
        }

        public static IEnumerable<TestSetting> CreateTestSettings3()
        {
            return 
                from setting in ReadTestSettings()
                let tags = new Dictionary<string, string>
                {
                    ["Environment"] = "Test",
                    ["Config"] = "TestSetting3"
                }
                select new TestSetting
                {
                    Name = setting.Key,
                    Value = setting.Value,
                    Tags = tags
                };
        }

        private static Dictionary<string, string> ReadTestSettings()
        {
            var json = ResourceReader.ReadEmbeddedResource<TestSettingFactory>("TestSettings.json");
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return settings;
        }
    }
}