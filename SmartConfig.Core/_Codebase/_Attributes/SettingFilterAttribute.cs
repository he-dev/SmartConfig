using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Filters;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingFilterAttribute : Attribute
    {
        public SettingFilterAttribute(Type filterType)
        {
            if (filterType == null)
            {
                throw new ArgumentNullException(nameof(filterType));
            }

            if (!typeof(ISettingFilter).IsAssignableFrom(filterType))
            {
                throw new TypeDoesNotImplementInterfaceException
                {
                    ExpectedType = typeof(ISettingFilter).FullName,
                    ActualType = filterType.FullName
                };
            }

            FilterType = filterType;
        }

        public Type FilterType { get; }
    }
}
