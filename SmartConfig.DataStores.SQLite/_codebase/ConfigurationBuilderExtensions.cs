using System;
using System.Data;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    public static class ConfigurationBuilderExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static ConfigurationBuilder FromSQLite(
            this ConfigurationBuilder configurationBuilder,
            string nameOrConnectionString,
            Action<TableMetadataBuilder<DbType>> configure = null
        )
        {
            return configurationBuilder.From(new SQLiteStore(nameOrConnectionString, configure));
        }
    }
}