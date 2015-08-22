using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public static class Logger
    {
        public static Action<string> Trace { get; set; }
        public static Action<string> Debug { get; set; }
        public static Action<string> Info { get; set; }
        public static Action<string> Warn { get; set; }

        internal static void LogTrace(Func<string> message)
        {
            if (Trace != null) Trace(message());
        }

        internal static void LogDebug(Func<string> message)
        {
            if (Debug != null) Debug(message());
        }

        internal static void LogInfo(Func<string> message)
        {
            if (Info != null) Info(message());
        }

        internal static void LogWarn(Func<string> message)
        {
            if (Warn != null) Warn(message());
        }
    }
}
