using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class SmartConfigLoader
    {
        private SmartConfigLoader() { }

        public static SmartConfigLoader From<TDataSource>(Action<TDataSource> dataSourceConfiguration) where TDataSource : IDataSource, new()
        {
            var dataSource = new TDataSource();
            return null;
        }

        public void Load(Type configType)
        {

        }
    }
}
