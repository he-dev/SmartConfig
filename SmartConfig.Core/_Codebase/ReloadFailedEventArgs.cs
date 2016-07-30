using System;

namespace SmartConfig
{
    public class ReloadFailedEventArgs : EventArgs
    {
        public Type ConfigurationType { get; internal set; }
        public Exception Exception { get; internal set; }
    }
}
