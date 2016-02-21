using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Filters
{
    public interface ISettingFilter
    {
        IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, SimpleSettingKey key);
    }
}
