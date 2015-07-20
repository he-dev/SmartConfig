namespace SmartConfig.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ConfigElement")]
    public partial class ConfigElement
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string Environment { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Version { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string Name { get; set; }

        [MaxLength]
        public string Value { get; set; }
    }
}
