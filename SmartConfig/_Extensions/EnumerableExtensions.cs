using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public static class EnumerableExtensions
    {
        public static void Check<T>(this IEnumerable<ValueConstraintAttribute> contraints, Action<T> checkAction) where T : ValueConstraintAttribute
        {
            var constraint = contraints.OfType<T>().SingleOrDefault();
            if (constraint != null)
            {
                checkAction(constraint);
            }
        }
    }
}
