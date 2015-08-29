using System;

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
            Trace?.Invoke(message());
        }

        internal static void LogDebug(Func<string> message)
        {
            Debug?.Invoke(message());
        }

        internal static void LogInfo(Func<string> message)
        {
            Info?.Invoke(message());
        }

        internal static void LogWarn(Func<string> message)
        {
            Warn?.Invoke(message());
        }
    }
}
