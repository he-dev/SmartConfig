using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public interface IDataSourceCollection : ICollection<Type, IDataSource>
    {

    }
}
