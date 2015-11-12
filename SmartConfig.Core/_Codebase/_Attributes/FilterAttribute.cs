using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Filters;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        public FilterAttribute(Type filterType)
        {
            if (filterType == null)
            {
                throw new ArgumentNullException(nameof(filterType));
            }

            if (!typeof(ISettingFilter).IsAssignableFrom(filterType))
            {
                throw new FilterTypeNotISettingFilterException { FilterTypeFullName = filterType.FullName };
            }

            FilterType = filterType;
        }

        public Type FilterType { get; }
    }
}
