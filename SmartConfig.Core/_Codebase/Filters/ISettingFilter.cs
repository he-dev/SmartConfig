using System.Collections.Generic;
using SmartConfig.Data;

namespace SmartConfig.Filters
{
    public interface ISettingFilter
    {
        IEnumerable<IIndexable> Apply(IEnumerable<IIndexable> settings, KeyValuePair<string, object> key);
    }
}
