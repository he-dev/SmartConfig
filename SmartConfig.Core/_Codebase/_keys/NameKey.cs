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
        

        private readonly SimpleSettingKey _simpleKey;

        public NameKey(SimpleSettingKey simpleKey)
        {
            if (simpleKey == null) { throw new ArgumentNullException(nameof(simpleKey)); }
            if (!(simpleKey.Value is SettingPath))
            {
                throw new InvalidOperationException($"{nameof(simpleKey)}.{nameof(SimpleSettingKey.Value)} must be of type {nameof(SettingPath)}.");
            }
            _simpleKey = simpleKey;
        }

        public string Name => nameof(Data.BasicSetting.Name);

        public SettingPath Value => (SettingPath)_simpleKey.Value;

        public static implicit operator string(NameKey nameKey)
        {
            Debug.Assert(nameKey._simpleKey.Value is SettingPath, "NameKey.Value must be of type SettingPath.");
            return nameKey._simpleKey.Value.ToString();
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

        protected bool Equals(NameKey other)
        {
            return Equals(_simpleKey, other._simpleKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NameKey)obj);
        }

        public override int GetHashCode()
        {
            return _simpleKey?.GetHashCode() ?? 0;
        }
    }
}
