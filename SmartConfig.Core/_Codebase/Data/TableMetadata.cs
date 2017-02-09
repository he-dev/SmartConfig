using System.Collections.Generic;
using System.Data;

namespace SmartConfig.Data
{
    public class TableMetadata<TDbType>
    {
        internal TableMetadata(string schemaName, string tableName, IReadOnlyDictionary<string, ColumnMetadata<TDbType>> columns)
        {
            SchemaName = schemaName;
            TableName = tableName;
            Columns = columns;
        }

        public string SchemaName { get; }

        public string TableName { get; }

        public IReadOnlyDictionary<string, ColumnMetadata<TDbType>> Columns { get; }
    }

    public class ColumnMetadata<TDbType>
    {        
        public ColumnMetadata(string name, TDbType dbType, int length)
        {
            Name = name;
            DbType = dbType;
            Length = length;
        }
        public string Name { get; }
        public TDbType DbType { get; set; }
        public int Length { get; set; }
    }
}