﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class ConfigurationReloadFailedEventArgs : EventArgs
    {
        public Exception Exception { get; internal set; }
    }
}
