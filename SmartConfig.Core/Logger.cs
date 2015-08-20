using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public static class Logger
    {
        public static Action<string> Log { get; set; }

        internal static void LogAction(Func<string> message)
        {
            if (Log != null) Log(message());
        }
    }
}
