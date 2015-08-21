using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class CompositeKey : Dictionary<string, string>
    {
        public CompositeKey(string defaultKeyValue, IEnumerable<string> keyNames, IDictionary<string, KeyProperties> keyProperties)
        {
            this[KeyNames.DefaultKeyName] = defaultKeyValue;

            foreach (var keyName in keyNames.Where(k => k != KeyNames.DefaultKeyName))
            {
                this[keyName] = keyProperties[keyName].Value;
            }
        }        
    }
}
