using System;
using System.Collections;
using System.Collections.Generic;
using SmartConfig.Data;

namespace SmartConfig.Collections
{
    public class TagCollection : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly IDictionary<string, object> _tags = new SortedDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public TagCollection() { }

        public TagCollection(IDictionary<string, object> tags)
        {
            foreach (var tag in tags)
            {
                Add(tag.Key, tag.Value);
            }
        }

        public object this[string key]
        {
            get { return _tags[key]; }
            set { Add(key, value); }
        }

        public IEnumerable<string> Keys => _tags.Keys;

        public int Count => _tags.Count;

        public void Add(string tag, object value)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentNullException(nameof(tag));
            if (value == null) throw new ArgumentNullException(nameof(value));

            _tags.Add(ThrowIfReservedName(tag.Trim()), value);
        }

        private static string ThrowIfReservedName(string tag)
        {
            var isReservedName =
                tag.Equals(nameof(Setting.Name), StringComparison.OrdinalIgnoreCase) ||
                tag.Equals(nameof(Setting.Value), StringComparison.OrdinalIgnoreCase);

            if (isReservedName)
                throw new ArgumentException($"'{nameof(Setting.Name)}' and '{nameof(Setting.Value)}' are reserved names.");

            return tag;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _tags.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}