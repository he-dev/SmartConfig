using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class CompositeKey : Dictionary<string, string>
    {
        public static CompositeKey From(string defaultKeyValue, IEnumerable<string> keyNames, IDictionary<string, KeyProperties> keyProperties)
        {
            var compositeKey = new CompositeKey()
            {
                { KeyNames.DefaultKeyName, defaultKeyValue }
            };

            foreach (var keyName in keyNames.Where(k => k != KeyNames.DefaultKeyName))
            {
                compositeKey[keyName] = keyProperties[keyName].Value;
            }

            return compositeKey;
        }
    }
}
