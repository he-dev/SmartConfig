using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        public FilterAttribute(Type filterType)
        {
            if (filterType == null) { throw new ArgumentNullException(nameof(filterType)); }
            
            FilterType = filterType;
        }

        public Type FilterType { get; }
    }
}
