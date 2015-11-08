using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartConfig
{
    public class CompositeKey : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _keys = new Dictionary<string, string>();

        internal CompositeKey(string defaultKeyValue, IEnumerable<string> keyNames, IDictionary<string, CustomKey> customKeys)
        {
            // add default key first
            _keys[KeyNames.DefaultKeyName] = defaultKeyValue;

            // add other keys but the default one
            foreach (var keyName in keyNames.Where(k => k != KeyNames.DefaultKeyName))
            {
                _keys[keyName] = customKeys[keyName].Value;
            }
        }

        public string this[string keyName] => _keys[keyName];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
