using System;
using System.Collections.Generic;

namespace SmartConfig.Data.SqlClient.Tests
{
    public class TestDataSource : DataSource<TestSetting>
    {
        public Func<string, string> SelectFunc;

        public Action<string, string> UpdateAction;

        public override string Select(string defaultKey)
        {
            return SelectFunc(defaultKey);
        }

        public override void Update(string defaultKey, string value)
        {
            UpdateAction(defaultKey, value);
        }
    }
}
