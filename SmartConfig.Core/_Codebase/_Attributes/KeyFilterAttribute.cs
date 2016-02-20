using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Filters;

namespace SmartConfig
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyFilterAttribute : Attribute
    {
        public KeyFilterAttribute(Type filterType)
        {
            if (filterType == null)
            {
                throw new ArgumentNullException(nameof(filterType));
            }

            if (!typeof(IKeyFilter).IsAssignableFrom(filterType))
            {
                throw new TypeDoesNotImplementInterfaceException
                {
                    ExpectedType = typeof(IKeyFilter).FullName,
                    ActualType = filterType.FullName
                };
            }

            FilterType = filterType;
        }

        public Type FilterType { get; }
    }
}
