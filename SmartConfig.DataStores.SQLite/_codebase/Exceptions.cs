using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartUtilities;

namespace SmartConfig.DataStores.SQLite
{
    internal class Exceptions
    {
        [Serializable]
        public class ConnectionStringNotFoundException : FormattableException
        {
            public string ConnectionStringName { get; internal set; }
        }
    }
}