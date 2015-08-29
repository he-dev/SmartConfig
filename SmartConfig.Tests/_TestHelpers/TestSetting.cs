using SmartConfig.Data;

namespace SmartConfig.Tests
{
    public class TestSetting : Setting
    {
        public TestSetting()  { }

        public TestSetting(string values) 
        {
            var columns = values.Split('|');
            Environment = columns[0];
            Version = columns[1];
            Name = columns[2];
            Value = columns[3];
        }

        public string Environment { get; set; }

        public string Version { get; set; }
    }
}
