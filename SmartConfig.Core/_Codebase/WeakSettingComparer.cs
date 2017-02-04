using System.Collections.Generic;
using System.Linq;
using SmartConfig.Data;

namespace SmartConfig
{
    // Compares settings by the weak-full-name and tags if available.
    public class WeakSettingComparer : IEqualityComparer<Setting>
    {
        public bool Equals(Setting x, Setting y)
        {
            return
                !ReferenceEquals(x, null) &&
                !ReferenceEquals(y, null) &&
                CreateWeakId(x) == CreateWeakId(y);
        }

        public int GetHashCode(Setting obj)
        {
            return CreateWeakId(obj).GetHashCode();
        }

        private static string CreateWeakId(Setting setting)
        {
            var tagString = string.Join(", ", setting.Tags.Select(x => x.Value));
            return $"[{setting.Name.WeakFullName}{(string.IsNullOrEmpty(tagString) ? string.Empty : $", {tagString}")}]";
        }
    }
}