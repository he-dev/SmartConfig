using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class RangeAttribute : ValueConstraintAttribute
    {
        private readonly Type _type;
        private readonly string _min;
        private readonly string _max;

        public RangeAttribute(Type type, string min, string max)
        {
            _type = type;
            _min = min;
            _max = max;
        }

        public bool IsValid(IComparable value)
        {
            var typeConverter = TypeDescriptor.GetConverter(_type);

            var isMin = true;
            if (!string.IsNullOrEmpty(_min))
            {
                var min = (IComparable)typeConverter.ConvertFromString(_min);
                isMin = value.CompareTo(min) >= 0;
            }

            var isMax = true;
            if (!string.IsNullOrEmpty(_max))
            {
                var max = (IComparable)typeConverter.ConvertFromString(_max);
                isMax = value.CompareTo(max) <= 0;
            }

            return isMin && isMax;
        }
    }
}
