using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Data.SqlClient.Tests
{
    public class TestConfigElement : ConfigElement
    {
        public TestConfigElement() { }

        public TestConfigElement(string values)
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
