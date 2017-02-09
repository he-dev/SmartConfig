using System;
using System.Data;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    public static class ConfigurationBuilderExtensions
    {
        public static ConfigurationBuilder FromSqlServer(
            this ConfigurationBuilder configurationBuilder,
            string nameOrConnectionString,
            Action<TableMetadataBuilder<SqlDbType>> configure = null
        )
        {
            return configurationBuilder.From(new SqlServerStore(nameOrConnectionString, configure));
        }
    }
}