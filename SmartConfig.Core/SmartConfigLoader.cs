using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class SmartConfigLoader
    {
        private SmartConfigLoader() { }

        public static SmartConfigLoader From<T>(Action<T> dataSourceConfiguration) where T : new ()
        {
            var dataSource = new T();
            return null;
        }

        public void Load(Type configType)
        {
            
        }
    }
}
