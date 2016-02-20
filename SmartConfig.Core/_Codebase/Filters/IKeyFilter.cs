using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig.Filters
{
    public interface IKeyFilter
    {
        IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, SettingKey key);
    }
}
