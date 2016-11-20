using System;
using System.Data;
using System.Linq.Expressions;
using Reusable;
using Reusable.Collections;
using Reusable.Validations;

namespace SmartConfig.DataStores.SQLite
{
    public class TableConfigurationBuilder
    {
        private string _tableName;
        private AutoKeyDictionary<string, ColumnConfiguration> _columnConfigurations = new AutoKeyDictionary<string, ColumnConfiguration>(x => x.Name);

        internal TableConfigurationBuilder() { }

        public TableConfigurationBuilder TableName(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        public TableConfigurationBuilder Column(
            string columnName, 
            DbType dbType = ColumnConfiguration.DefaultDbType, 
            int length = ColumnConfiguration.DefaultLength)
        {
            var columnConfiguration = new ColumnConfiguration(columnName, dbType, length);
            _columnConfigurations.Remove(columnConfiguration);
            _columnConfigurations.Add(columnConfiguration);
            return this;
        }

        public TableConfigurationBuilder Column<T>(
            Expression<Func<T>> expression,
             DbType dbType = ColumnConfiguration.DefaultDbType,
            int length = ColumnConfiguration.DefaultLength)
        {
            expression.Validate(nameof(expression)).IsNotNull();
            var memberExpression = (expression.Body as MemberExpression).Validate().IsNotNull($"The expression must be a {nameof(MemberExpression)}.").Value;
            return Column(memberExpression.Member.Name, dbType, length);
        }

        internal TableConfiguration Build()
        {
            return new TableConfiguration(_tableName, _columnConfigurations);
        }
    }
}