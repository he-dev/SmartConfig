using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Makes checking constrains easier. It looks for the specified constraint and if found one calls the action.
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
