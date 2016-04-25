using System;
using SmartConfig.Filters;
using SmartUtilities;

namespace SmartConfig.DataAnnotations
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

            if (!filterType.Implements<ISettingFilter>())
            {
                throw new ArgumentException($"Filter {filterType.Name} must implement the {typeof(ISettingFilter).Name} interface.");
            }

            if (!filterType.HasDefaultConstructor())
            {
                throw new ArgumentException($"Filter {filterType.Name} must provide a default constructor.");
            }

            FilterType = filterType;
        }

        public Type FilterType { get; }
    }
}
