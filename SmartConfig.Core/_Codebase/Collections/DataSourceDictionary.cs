using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Converters;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Collections
{
    /// <summary>
    /// Maps config types to their data sources.
    /// </summary>
    public sealed class DataSourceDictionary : Dictionary<Type, IDataSource>
    {
        // don't let create this dictionary outside the assembly
        internal DataSourceDictionary() { }

        /// <summary>
        /// Gets a data source for the specified config type.
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        new public IDataSource this[Type configType]
        {
            get
            {
                Debug.Assert(configType != null);

                IDataSource dataSource;
                if (!TryGetValue(configType, out dataSource))
                {
                    // looks like the specified config type is not initialized
                    throw new InvalidOperationException("Data source for config [$ConfigTypeName] not found. Did you forget to initialize it?".FormatWith(new { ConfigTypeName = configType.Name }));
                }
                return dataSource;
            }
            set { ((Dictionary<Type, IDataSource>)this)[configType] = value; }
        }        
    }
}
