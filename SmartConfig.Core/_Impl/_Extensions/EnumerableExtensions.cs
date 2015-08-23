using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Allows to check a constraint if it exists.
        /// </summary>
        /// <typeparam name="TConstraint"></typeparam>
        /// <param name="contraints"></param>
        /// <param name="checkAction"></param>
        public static void Check<TConstraint>(this IEnumerable<ConstraintAttribute> contraints, Action<TConstraint> checkAction) where TConstraint : ConstraintAttribute
        {
            var constraint = contraints.OfType<TConstraint>().SingleOrDefault();
            if (constraint != null)
            {
                checkAction(constraint);
            }
        }       
    }
}
