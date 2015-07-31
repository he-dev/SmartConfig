using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Example.Data
{
    public partial class CustomConfigElement : BasicConfigElement, IEnvironment, IVersion
    {
        [Key]
        [StringLength(50)]
        [Column(Order = 2)]
        public string Environment { get; set; }

        [Key]
        [StringLength(50)]
        [Column(Order = 3)]
        public string Version { get; set; }
    }
}
