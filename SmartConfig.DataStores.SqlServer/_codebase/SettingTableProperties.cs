using System;
using System.Collections.Generic;
using System.Data;

namespace SmartConfig.DataStores.SqlServer
{
    public class SettingTableProperties
    {
        private static readonly SqlDbType DefaultSqlDbType = SqlDbType.NVarChar;

        private static readonly int DefaultColumnLength = 50;

        private readonly Dictionary<string, SqlDbType> _sqlDbTypes = new Dictionary<string, SqlDbType>();

        private readonly Dictionary<string, int> _columnLengths = new Dictionary<string, int>();

        public static readonly SettingTableProperties Default = new Builder().ToSettingTableProperties();

        private SettingTableProperties() { }

        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public IReadOnlyDictionary<string, SqlDbType> SqlDbTypes => _sqlDbTypes;

        public IReadOnlyDictionary<string, int> ColumnLengths => _columnLengths;

        public SqlDbType GetSqlDbTypeOrDefault(string name)
        {
            SqlDbType result;
            return SqlDbTypes.TryGetValue(name, out result) ? result : DefaultSqlDbType;
        }

        public int GetColumnLengthOrDefault(string name)
        {
            int result;
            return ColumnLengths.TryGetValue(name, out result) ? result : DefaultColumnLength;
        }

        public Type MapDataType(Type objectType) => typeof(string);

        //public static SettingTableProperties Build(Action<Builder> build)
        //{
        //    var builder = new Builder();
        //    build(builder);
        //    return builder.ToSettingTableProperties();
        //}

        public class Builder
        {
            private SettingTableProperties _properties = new SettingTableProperties();

            private const int NVarCharMax = -1;

            internal Builder()
            {
                SchemaName("dbo");
                TableName("Setting");
                ColumnProperties("Name", SqlDbType.NVarChar, 200);
                ColumnProperties("Value", SqlDbType.NVarChar, NVarCharMax);
            }

            public Builder SchemaName(string schemaName)
            {
                _properties.SchemaName = schemaName;
                return this;
            }

            public Builder TableName(string tableName)
            {
                _properties.TableName = tableName;
                return this;
            }

            public Builder ColumnProperties(string columnName, SqlDbType sqlDbType, int length)
            {
                _properties._sqlDbTypes[columnName] = sqlDbType;
                _properties._columnLengths[columnName] = length;
                return this;
            }

            public Builder ColumnProperties(string columnName, SqlDbType sqlDbType)
            {
                _properties._sqlDbTypes[columnName] = sqlDbType;
                return this;
            }

            internal SettingTableProperties ToSettingTableProperties()
            {
                var result = _properties;
                _properties = null;
                return result;
            }
        }
    }
}