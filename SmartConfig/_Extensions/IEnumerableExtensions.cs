using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    internal static class IEnumerableExtensions
    {
        public static bool AllowNull(this IEnumerable<ValueContraintAttribute> constraints)
        {
            var allowNull = constraints.OfType<AllowNullAttribute>().SingleOrDefault();
            return allowNull == null;
        }
    }
}
