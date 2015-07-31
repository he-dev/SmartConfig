using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartConfig.Data
{
    public partial class BasicConfigElement
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(255)]
        public string Name { get; set; }

        [MaxLength]
        public string Value { get; set; }
    }
}
