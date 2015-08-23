using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SmartConfig.Data;

namespace SmartConfig
{
    public class KeyNames : List<string>
    {
        public const string DefaultKeyName = "Name";
        public const string EnvironmentKeyName = "Environment";
        public const string VersionKeyName = "Version";
        public const string MachineNameKeyName = "MachineName";
        public const string UserNameKeyName = "UserName";

        internal static class Internal
        {
            internal const string SettingsInitializedKeyName = "__SettingsInitialized";
        }

        public static KeyNames From<TSetting>() where TSetting : Setting
        {
            var keyMembers = new KeyNames()
            {
                DefaultKeyName
            };

            var currentType = typeof(TSetting);

            var isCustomType = currentType != typeof(Setting);
            if (!isCustomType)
            {
                return keyMembers;
            }

            var propertyNames =
                currentType
                    .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => p.Name)
                    .OrderBy(n => n);
            keyMembers.AddRange(propertyNames);

            return keyMembers;
        }
    }
}
