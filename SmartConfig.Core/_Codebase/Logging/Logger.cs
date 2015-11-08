using System;
using System.Runtime.CompilerServices;

namespace SmartConfig.Logging
{
    public static class Logger
    {
        public static Action<string> Trace { get; set; }
        public static Action<string> Debug { get; set; }
        public static Action<string> Info { get; set; }
        public static Action<string> Warn { get; set; }

        internal static void Log(LogLevel logLevel, Func<string> message, [CallerMemberName] string methodName = "")
        { }

        internal static void LogTrace(Func<string> message, [CallerMemberName] string methodName = "")
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
