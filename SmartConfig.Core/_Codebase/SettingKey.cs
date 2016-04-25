using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SmartConfig.Data;

namespace SmartConfig
{
    [DebuggerDisplay("{ToString()}")]
    public class SettingKey : IEnumerable<KeyValuePair<string, object>> //: ReadOnlyCollection<SimpleSettingKey>
    {
        private readonly IDictionary<string, object> _keys = new Dictionary<string, object>();

        internal SettingKey(SettingPath path, IEnumerable<KeyValuePair<string, object>> customKeys)
        {
            _keys[BasicSetting.DefaultKeyName] = path;

            // customKeys are ordered alphabeticaly
            foreach (var item in customKeys.OrderBy(x => x.Key))
            {
                _keys[item.Key] = item.Value;
            }
        }

        public object this[string keyName] => _keys[keyName];

        public KeyValuePair<string, SettingPath> Name => new KeyValuePair<string, SettingPath>
        (
            BasicSetting.DefaultKeyName,
            (SettingPath)_keys[BasicSetting.DefaultKeyName]
        );

        public IDictionary<string, object> CustomKeys => _keys.Where(x => x.Key != BasicSetting.DefaultKeyName).ToDictionary(x => x.Key, x => x.Value);

        public static SettingKey From(SettingPath path, IEnumerable<KeyValuePair<string, object>> otherKeys)
        {
            return new SettingKey(path, otherKeys);
        }

        public static SettingKey From<TSetting>() where TSetting : BasicSetting, new()
        {
            var setting = new TSetting();
            var customKeys = setting.CustomKeyProperties.Select(property => new KeyValuePair<string, object>(property.Name, null));

            return new SettingKey(null, customKeys);
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(" ", _keys.Select(x => $"{x.Key} = '{x.Value}'"));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
