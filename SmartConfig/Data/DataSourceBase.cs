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
        public abstract IEnumerable<ConfigElement> Select(string name);

        public abstract void Update(ConfigElement configElement);
    }
}