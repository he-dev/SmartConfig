﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Data
{
    /// <summary>
    /// Provides methods for selecting and updating data.
    /// </summary>
    public abstract class DataSourceBase
    {
        public abstract string Select(IDictionary<string, string> keys);

        public abstract void Update(IDictionary<string, string> keys, string value);
    }   
}