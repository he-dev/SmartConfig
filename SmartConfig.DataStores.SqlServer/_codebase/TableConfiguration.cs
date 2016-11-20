using System;
using System.Collections.Generic;
using System.Data;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    public class TableConfiguration
    {
        internal TableConfiguration(string schemaName, string tableName, IReadOnlyDictionary<string, ColumnConfiguration> columns)
        {
            SchemaName = schemaName;
            TableName = tableName;
            Columns = columns;
        }

        internal string SchemaName { get; }

        internal string TableName { get; }

        internal IReadOnlyDictionary<string, ColumnConfiguration> Columns { get; }
    }

    internal class ColumnConfiguration
    {
        public const SqlDbType DefaultDbType = SqlDbType.NVarChar;
        public const int MaxLength = -1;
        public const int DefaultLength = 50;

        public ColumnConfiguration(string name, SqlDbType dbType, int length)
        {
            Name = name;
            DbType = dbType;
            Length = length;
        }
        public string Name { get; }
        public SqlDbType DbType { get; set; }
        public int Length { get; set; }
    }
}