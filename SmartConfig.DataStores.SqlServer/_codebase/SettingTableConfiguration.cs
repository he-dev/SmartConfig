using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Reusable;
using Reusable.Validations;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    public class SettingTableConfiguration
    {
        private const int NVarCharMax = -1;

        internal SettingTableConfiguration()
        {
            Columns = new Dictionary<string, ColumnConfiguration>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Setting.Name)] = new ColumnConfiguration
                {
                    DbType = SqlDbType.NVarChar,
                    Length = 300
                },
                [nameof(Setting.Value)] = new ColumnConfiguration
                {
                    DbType = SqlDbType.NVarChar,
                    Length = NVarCharMax
                },
                [nameof(Setting.Config)] = new ColumnConfiguration
                {
                    DbType = SqlDbType.NVarChar,
                    Length = 300
                }
            };
        }

        internal string SchemaName { get; private set; } = "dbo";

        internal string TableName { get; private set; } = nameof(Setting);

        internal Dictionary<string, ColumnConfiguration> Columns { get; }

        public class Builder
        {
            private readonly SettingTableConfiguration _settingTableConfiguration = new SettingTableConfiguration();

            internal Builder() { }

            public Builder SchemaName(string schemaName)
            {
                _settingTableConfiguration.SchemaName = schemaName;
                return this;
            }

            public Builder TableName(string tableName)
            {
                _settingTableConfiguration.TableName = tableName;
                return this;
            }

            public Builder Column(string columnName, SqlDbType sqlDbType = SqlDbType.NVarChar, int length = 50)
            {
                _settingTableConfiguration.Columns[columnName] = new ColumnConfiguration
                {
                    DbType = sqlDbType,
                    Length = length
                };
                return this;
            }

            public Builder Column<T>(Expression<Func<T>> expression, SqlDbType sqlDbType = SqlDbType.NVarChar, int length = 50)
            {
                expression.Validate(nameof(expression)).IsNotNull();
                var memberExpression = (expression.Body as MemberExpression).Validate().IsNotNull($"The expression must be a {nameof(MemberExpression)}.").Value;
                return Column(memberExpression.Member.Name, sqlDbType, length);
            }

            internal SettingTableConfiguration Build()
            {
                return _settingTableConfiguration;
            }
        }
    }

    internal class ColumnConfiguration
    {
        public SqlDbType DbType { get; set; }
        public int Length { get; set; }
    }
}