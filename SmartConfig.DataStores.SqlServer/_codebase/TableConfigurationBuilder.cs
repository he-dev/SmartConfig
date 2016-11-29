using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Reusable;
using Reusable.Collections;
using Reusable.Fuse;

namespace SmartConfig.DataStores.SqlServer
{
    public class TableConfigurationBuilder
    {
        private string _schemaName;
        private string _tableName;
        private AutoKeyDictionary<string, ColumnConfiguration> _columns = new AutoKeyDictionary<string, ColumnConfiguration>(x => x.Name, StringComparer.OrdinalIgnoreCase);

        internal TableConfigurationBuilder() { }

        public TableConfigurationBuilder SchemaName(string schemaName)
        {
            _schemaName = schemaName;
            return this;
        }

        public TableConfigurationBuilder TableName(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public TableConfigurationBuilder Column(
            string name,
            SqlDbType sqlDbType = ColumnConfiguration.DefaultDbType,
            int length = ColumnConfiguration.DefaultLength)
        {
            var column = new ColumnConfiguration(name, sqlDbType, length);
            _columns.Remove(column);
            _columns.Add(column);
            return this;
        }

        public TableConfigurationBuilder Column<T>(
            Expression<Func<T>> expression,
            SqlDbType sqlDbType = ColumnConfiguration.DefaultDbType,
            int length = ColumnConfiguration.DefaultLength)
        {
            expression.Validate(nameof(expression)).IsNotNull();
            var memberExpression = (expression.Body as MemberExpression).Validate().IsNotNull($"The expression must be a {nameof(MemberExpression)}.").Value;
            return Column(memberExpression.Member.Name, sqlDbType, length);
        }

        internal TableConfiguration Build()
        {
            return new TableConfiguration(_schemaName, _tableName, _columns);
        }
    }
}