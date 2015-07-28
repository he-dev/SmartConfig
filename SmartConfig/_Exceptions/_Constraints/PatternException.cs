using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class PatternException : ConstraintException
    {
        public PatternException(string value, string pattern)
            : base(value, string.Format("Value [{0}] does not match the pattern [{1}].", value, pattern))
        {
            Value = value;
            Pattern = pattern;
        }

        public string Pattern { get; private set; }
    }
}
