using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Filters
{
    // sorts an array so that the * is at the end an the order of other items is maintained
    public class WildcardComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
        {
            if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
            {
                return 0;
            }

            var asterisk = Wildcards.Asterisk[0];

            var chars = new[] { x[0], y[0] };

            if (chars.All(c => c != asterisk) || chars.All(c => c == asterisk))
            {
                return 0;
            }

            if (chars.Any(c => c == asterisk))
            {
                if (x[0] == asterisk)
                {
                    return 1;
                }
                if (y[0] == asterisk)
                {
                    return -1;
                }
            }

            return 0;
        }
    }
}
