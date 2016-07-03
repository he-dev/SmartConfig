using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SettingKey : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _keys = new Dictionary<string, object>();

        internal SettingKey(SettingPath path, IEnumerable<KeyValuePair<string, object>> customKeyValuePairs)
        {
            _keys[BasicSetting.MainKeyName] = path;
            foreach (var item in customKeyValuePairs)
            {
                _keys[item.Key] = item.Value;
            }
        }

        public object this[string keyName] => _keys[keyName];

        public KeyValuePair<string, SettingPath> Main
        {
            get
            {
                var main = _keys.First();
                return new KeyValuePair<string, SettingPath>(main.Key, (SettingPath)main.Value);
            }
        }

        public IDictionary<string, object> CustomKeys => _keys.Where(x => x.Key != BasicSetting.MainKeyName).ToDictionary(x => x.Key, x => x.Value);

        public static SettingKey From(SettingPath path, IEnumerable<KeyValuePair<string, object>> otherKeys)
        {
            return new SettingKey(path, otherKeys);
        }

        public static SettingKey From<TSetting>() where TSetting : BasicSetting, new()
        {
            var setting = new TSetting();
            var customKeys = setting.CustomKeyNames.Select(name => new KeyValuePair<string, object>(name, null));
            return new SettingKey(null, customKeys);
        }

        private string DebuggerDisplay
        {
            get { return string.Join(" ", _keys.Select(x => $"{x.Key} = '{x.Value}'")); }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
