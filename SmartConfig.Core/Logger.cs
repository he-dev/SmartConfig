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

        public static bool Enabled { get; set; }

        static Logger()
        {
#if DEBUG
            Enabled = true;
#endif
        }

        internal static void LogAction(Func<string> message)
        {
            if (Enabled && Log != null) Log(message());
        }
    }
}
