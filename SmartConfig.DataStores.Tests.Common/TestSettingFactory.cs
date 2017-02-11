using System.Collections.Generic;
using Newtonsoft.Json;
using Reusable;

namespace SmartConfig.DataStores.Tests.Common
{
    public class TestSettingFactory
    {
        public static IEnumerable<TestSetting> CreateTestSettings(IDictionary<string, string> tags = null)
        {
            tags = tags ?? new Dictionary<string, string>();

            var json = ResourceReader.ReadEmbededResource<TestSettingFactory, TestSettingFactory>("TestSettings.json");
            var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            foreach (var setting in settings)
            {
                yield return new TestSetting
                {
                    Name = setting.Key,
                    Value = setting.Value,
                    Tags = tags
                };
            }            
        }
    }
}