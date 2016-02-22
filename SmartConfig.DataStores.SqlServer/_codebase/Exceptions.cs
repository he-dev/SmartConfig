using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig.DataStores.SqlServer
{
    [Serializable]
    public class ConnectionStringNotFoundException : SmartException
    {
        public string ConnectionStringName { get { return GetValue<string>(); } internal set { SetValue(value); } }
    }
}
