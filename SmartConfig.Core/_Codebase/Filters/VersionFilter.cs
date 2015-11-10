﻿using System;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Filters
{
    /// <summary>
    /// Implements popular filters.
    /// </summary>
    public class VersionFilter : ISettingFilter
    {
        public IEnumerable<IIndexer> FilterSettings(IEnumerable<IIndexer> settings, SettingKey key)
        {
            var filtered = settings
                // get matching versions
                .Where(setting =>
                    !setting[key.Name].Equals(Wildcards.Asterisk) &&
                    SemanticVersion.Parse(setting[key.Name]) <= SemanticVersion.Parse(key.Value))
                // sort versions
                .OrderByDescending(setting => SemanticVersion.Parse(setting[key.Name]))
                // attach * at the end
                .Concat(settings.Where(setting => setting[key.Name].Equals(Wildcards.Asterisk)));

            // there can be only one version
            return filtered;
        }
    }
}