using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartConfig.Collections
{
    internal class SettingCollection : ReadOnlyCollection<Setting>
    {
        private SettingCollection(IList<Setting> list) : base(list) { }

        internal static SettingCollection Create(Configuration configuration)
        {
            var settingGroups = GetSettingGroups(configuration.Type);

            var settings = settingGroups
                .Select(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .SelectMany(sis => sis)
                .Where(p => !p.HasAttribute<IgnoreAttribute>())
                .Select(p => new Setting(p, configuration))
                .ToList();

            return new SettingCollection(settings);
        }

        internal static List<Type> GetSettingGroups(Type type, List<Type> settingGroups = null)
        {
            Debug.Assert(type != null);

            if (!type.IsStatic()) { throw new TypeNotStaticException { Type = type.FullName }; }

            settingGroups = settingGroups ?? new List<Type> { type }; ;

            var nestedSettingGroups = type
                .GetNestedTypes(BindingFlags.Public | BindingFlags.Public)
                .Where(t => !t.HasAttribute<IgnoreAttribute>());

            foreach (var nestedSettingGroup in nestedSettingGroups)
            {
                settingGroups.Add(nestedSettingGroup);
                GetSettingGroups(nestedSettingGroup, settingGroups);
            }

            return settingGroups;
        }
    }
}
