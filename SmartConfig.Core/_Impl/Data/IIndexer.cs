using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    public interface IIndexer
    {
        string this[string propertyName] { get; set; }
    }
}
