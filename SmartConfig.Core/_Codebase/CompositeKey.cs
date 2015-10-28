using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    public class CompositeKey : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> Keys = new Dictionary<string, string>();

        public CompositeKey(string defaultKeyValue, IEnumerable<string> keyNames, IDictionary<string, CustomKey> customKeys)
        {
            Keys[KeyNames.DefaultKeyName] = defaultKeyValue;

            foreach (var keyName in keyNames.Where(k => k != KeyNames.DefaultKeyName))
            {
                Keys[keyName] = customKeys[keyName].Value;
            }
        }

        public string this[string keyName] => Keys[keyName];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
