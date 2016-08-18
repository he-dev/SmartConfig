using System;
using System.Collections.Generic;
using System.Data;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SQLite
{
    public class SettingTableConfiguration
    {
        private const int StringMax = -1;

        internal SettingTableConfiguration()
        {
            Columns = new Dictionary<string, ColumnConfiguration>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Setting.Name)] = new ColumnConfiguration
                {
                    DbType = DbType.String,
                    Length = 300
                },
                [nameof(Setting.Value)] = new ColumnConfiguration
                {
                    DbType = DbType.String,
                    Length = StringMax
                },
                [nameof(Setting.Config)] = new ColumnConfiguration
                {
                    DbType = DbType.String,
                    Length = 300
                }
            };
        }

        internal string TableName { get; private set; } = nameof(Setting);

        internal Dictionary<string, ColumnConfiguration> Columns { get; }

        public class Builder
        {
            private readonly SettingTableConfiguration _settingTableConfiguration = new SettingTableConfiguration();

            internal Builder() { }

            public Builder TableName(string tableName)
            {
                _settingTableConfiguration.TableName = tableName;
                return this;
            }

            public Builder Column(string columnName, DbType dbType, int length)
            {
                _settingTableConfiguration.Columns[columnName] = new ColumnConfiguration
                {
                    DbType = dbType,
                    Length = length
                };
                return this;
            }

            internal SettingTableConfiguration Build()
            {
                return _settingTableConfiguration;
            }
        }
    }

    internal class ColumnConfiguration
    {
        public DbType DbType { get; set; }
        public int Length { get; set; }
    }    
}