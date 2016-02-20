using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig
{
    public class NameKey
    {
        private readonly SettingKey _settingKey;

        public NameKey(SettingKey settingKey)
        {
            if (settingKey == null) { throw new ArgumentNullException(nameof(settingKey)); }
            if (!(settingKey.Value is SettingPath))
            {
                throw new InvalidOperationException($"{nameof(settingKey)}.{nameof(SettingKey.Value)} must be of type {nameof(SettingPath)}.");
            }
            _settingKey = settingKey;
        }

        public string Name => nameof(Data.BasicSetting.Name);

        public SettingPath Value => (SettingPath)_settingKey.Value;

        public static implicit operator string(NameKey nameKey)
        {
            Debug.Assert(nameKey._settingKey.Value is SettingPath, "NameKey.Value must be of type SettingPath.");
            return nameKey._settingKey.Value.ToString();
        }

        public static bool operator ==(NameKey x, NameKey y)
        {
            return
                !ReferenceEquals(x, null) &&
                !ReferenceEquals(y, null) &&
                x.Name == y.Name &&
                x.Value == y.Value;
        }

        public static bool operator !=(NameKey x, NameKey y)
        {
            return !(x == y);
        }
    }
}
