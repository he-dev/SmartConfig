using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Reusable;
using Reusable.Collections;
using Reusable.Fuse;
using SmartConfig.Data;

namespace SmartConfig.DataStores.SqlServer
{
    public class SqlServerTableMetadataBuilder : TableMetadataBuilder<SqlDbType>
    {
        public static SqlServerTableMetadataBuilder Create() => new SqlServerTableMetadataBuilder();

        public override SqlServerTableMetadataBuilder Column(string name, SqlDbType sqlDbType, int length)
        {
            base.Column(name, sqlDbType, length);
            return this;
        }

        public SqlServerTableMetadataBuilder Column(string name, SqlDbType sqlDbType)
        {
            Column(name, sqlDbType, 50);
            return this;
        }

        public SqlServerTableMetadataBuilder Column(string name)
        {
            Column(name, SqlDbType.NVarChar, 50);
            return this;
        }
    }    
}