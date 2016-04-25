using System;

namespace SmartConfig
{
    public class ReloadFailedEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }
    }
}
