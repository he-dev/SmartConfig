using System;
using System.Collections.Generic;
using System.Data;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    public class TableConfiguration
    {
        internal TableConfiguration(string tableName, IReadOnlyDictionary<string, ColumnConfiguration> columns)
        {
            TableName = tableName;
            Columns = columns;
        }

        internal string TableName { get; }

        internal IReadOnlyDictionary<string, ColumnConfiguration> Columns { get; }
    }

    internal class ColumnConfiguration
    {
        public const DbType DefaultDbType = DbType.String;
        public const int MaxLength = -1;
        public const int DefaultLength = 50;

        public ColumnConfiguration(string name, DbType dbType, int length)
        {
            Name = name;
            DbType = dbType;
            Length = length;
        }

        public string Name { get; }
        public DbType DbType { get; }
        public int Length { get; }
    }
}