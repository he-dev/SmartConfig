using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Tests
{
    static class TestConfigHelper
    {
        public static List<ConfigElement> ToConfigElements(this IEnumerable<string> strings)
        {
            var testConfig =
                strings
                    .Select(s =>
                    {
                        var values = s.Split('|');
                        var cf = new ConfigElement()
                        {
                            Environment = values[0],
                            Version = values[1],
                            Name = values[2],
                            Value = values[3]
                        };
                        return cf;
                    }).ToList();
            return testConfig;
        }
    }
}
