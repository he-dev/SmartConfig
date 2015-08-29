using System.Collections.Generic;
using System.Linq;

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

        public string DefaultKey => this[KeyNames.DefaultKeyName];
    }
}
