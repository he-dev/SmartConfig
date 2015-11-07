using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using SmartConfig.Data;
using SmartUtilities;

namespace SmartConfig.Collections
{
    /// <summary>
    /// Maps config types to their data sources.
    /// </summary>
    public sealed class DataSourceCollection : IDataSourceCollection
    {
        private readonly Dictionary<Type, IDataSource> _dataSources = new Dictionary<Type, IDataSource>();

        // don't let create this dictionary outside the assembly
        internal DataSourceCollection() { }

        /// <summary>
        /// Gets a data source for the specified config type.
        /// </summary>
        /// <param name="configType"></param>
        /// <returns></returns>
        public IDataSource this[Type configType]
        {
            get
            {
                Debug.Assert(configType != null);

                IDataSource dataSource;
                if (!_dataSources.TryGetValue(configType, out dataSource))
                {
                    throw new DataSourceNotFoundException(configType);
                }
                return dataSource;
            }
            set { _dataSources[configType] = value; }
        }

        public IEnumerator<IDataSource> GetEnumerator()
        {
            return _dataSources.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

