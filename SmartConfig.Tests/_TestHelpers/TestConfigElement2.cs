using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Tests
{
    public class TestConfigElement2
    {
        public TestConfigElement2(string nameValue)
        {
            var columns = nameValue.Split('|');
            Name = columns[0];
            Value = columns[1];
        }

        public string Name { get; set; }
        
        public string Value { get; set; }
    }
}
