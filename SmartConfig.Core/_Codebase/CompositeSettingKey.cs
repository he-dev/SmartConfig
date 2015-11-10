using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmartConfig.Collections;

namespace SmartConfig
{
    public class CompositeSettingKey : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _keys = new Dictionary<string, string>();

        internal CompositeSettingKey(string defaultKeyValue, IEnumerable<string> keyNames, IDictionary<string, SettingKey> customKeys)
        {
            // add default key first
            _keys[SettingKeyNameReadOnlyCollection.DefaultKeyName] = defaultKeyValue;

            // add other keys but the default one
            foreach (var keyName in keyNames.Where(k => k != SettingKeyNameReadOnlyCollection.DefaultKeyName))
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
