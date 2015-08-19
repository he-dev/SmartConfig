using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class SmartConfigBuilder
    {
        private SmartConfigBuilder() { }

        public static SmartConfigBuilder From<TDataSource>(Action<IConfigureKey> dataSourceConfiguration) 
            where TDataSource : IDataSource, new()
        {
            var dataSource = new TDataSource();
            return null;
        }

        public void Load(Type configType)
        {

        }
    }
}
