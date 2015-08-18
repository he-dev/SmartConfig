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
    public class CustomSetting : Setting
    {
        public string Environment { get; set; }

        public string Version { get; set; }
    }
}
