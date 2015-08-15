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
    public class CustomConfigElement : ConfigElement
    {
        public CustomConfigElement() : base(typeof(CustomConfigElement)) { }

        public string Environment { get; set; }

        public string Version { get; set; }
    }
}
