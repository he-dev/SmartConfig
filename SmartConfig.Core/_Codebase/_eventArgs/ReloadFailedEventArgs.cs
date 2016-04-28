using System;

namespace SmartConfig
{
    public class ReloadFailedEventArgs : EventArgs
    {
        public string Configuration { get; set; }
        public Exception Exception { get; internal set; }
    }
}
