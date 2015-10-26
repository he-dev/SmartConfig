using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;

namespace SmartConfig.Collections
{
    public interface IObjectConverterCollection : ICollection<Type, ObjectConverter>
    {
    }
}
