using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartConfig.Data
{
    /// <summary>
    /// Basic config element with only two properties. Custom config elements must be derived from this type.
    /// </summary>
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
