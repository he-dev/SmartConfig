using System.Collections.Generic;

namespace SmartConfig.DataStores.Tests.Common
{
    public class TestSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
    }
}