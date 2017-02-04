using System;
using System.Collections.Generic;
using SmartConfig.Data;

namespace SmartConfig
{
    // Compares setting-pats by the weak-full-name.
    public class WeakSettingPathComparer : IEqualityComparer<SettingPath>
    {
        public bool Equals(SettingPath x, SettingPath y)
        {
            return
                !ReferenceEquals(x, null) &&
                !ReferenceEquals(y, null) &&
                x.WeakFullName.Equals(y.WeakFullName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(SettingPath obj)
        {
            return obj.WeakFullName.GetHashCode();
        }
    }
}