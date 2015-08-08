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
    public partial class TestConfigElement : BasicConfigElement, IEnvironment, ISemanticVersion
    {
        public TestConfigElement() { }

        public TestConfigElement(string values)
        {
            var columns = values.Split('|');
            Environment = columns[0];
            SemanticVersion = columns[1];
            Name = columns[2];
            Value = columns[3];
        }

        [Key]
        [StringLength(50)]
        [Column(Order = 2)]
        public string Environment { get; set; }

        [Key]
        [StringLength(50)]
        [Column(Order = 3)]
        public string SemanticVersion { get; set; }
    }
}
