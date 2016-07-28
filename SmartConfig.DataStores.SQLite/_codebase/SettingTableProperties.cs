using System.Collections.Generic;
using System.Data;

namespace SmartConfig.DataStores.SQLite
{
    public class SettingTableProperties
    {
        public string TableName { get; private set; }

        private static readonly DbType DefaultDbType = DbType.String;

        private static readonly int DefaultColumnLength = 50;

        private readonly Dictionary<string, DbType> _dbTypes = new Dictionary<string, DbType>();

        private readonly Dictionary<string, int> _columnLengths = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, DbType> DbTypes => _dbTypes;

        public IReadOnlyDictionary<string, int> ColumnLengths => _columnLengths;

        public DbType GetDbTypeOrDefault(string name)
        {
            DbType result;
            return DbTypes.TryGetValue(name, out result) ? result : DefaultDbType;
        }

        public int GetColumnLengthOrDefault(string name)
        {
            int result;
            return ColumnLengths.TryGetValue(name, out result) ? result : DefaultColumnLength;
        }

        public class Builder
        {
            private SettingTableProperties _properties = new SettingTableProperties();

            internal Builder()
            {
                TableName("Setting");
                ColumnProperties("Name", DbType.String, 200);
                ColumnProperties("Value", DbType.String, -1);
            }

            public Builder TableName(string tableName)
            {
                _properties.TableName = tableName;
                return this;
            }

            public Builder ColumnProperties(string columnName, DbType dbType, int length)
            {
                _properties._dbTypes[columnName] = dbType;
                _properties._columnLengths[columnName] = length;
                return this;
            }

            public Builder ColumnProperties(string columnName, DbType dbType)
            {
                _properties._dbTypes[columnName] = dbType;
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