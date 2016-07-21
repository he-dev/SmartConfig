using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using SmartConfig.Data;
using SmartUtilities;
using SmartUtilities.TypeFramework;

namespace SmartConfig.IO
{
    internal class SettingWriter
    {
        private readonly IDictionary<SettingInfo, object> _cache = new Dictionary<SettingInfo, object>();

        public SettingWriter(IDataStore dataStore, TypeConverter converter)
        {
            DataStore = dataStore;
            Converter = converter;
        }

        public IDataStore DataStore { get; }

        public TypeConverter Converter { get; set; }

        public Dictionary<string, object> Namespaces { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public int SaveSettings(IEnumerable<SettingInfo> settings)
        {
            _cache.Clear();
            var validationExceptions = new List<Exception>();

            foreach (var setting in settings)
            {
                try
                {
                    var dataType = DataStore.MapDataType(setting.Type);
                    var value = dataType == setting.Type ? setting.Value : Converter.Convert(setting.Value, dataType, CultureInfo.InvariantCulture);
                    _cache[setting] = value;
                }
                catch (Exception ex) { validationExceptions.Add(ex); }
            }
            return Commit();
        }

        private int Commit()
        {
            try
            {
                var settings = _cache.ToDictionary(x => x.Key.SettingPath, x => x.Value);
                var affectedSettings = DataStore.SaveSettings(settings, Namespaces);
                return affectedSettings;
            }
            catch (Exception ex)
            {
                throw new SaveSettingsException(ex) { DataStore = DataStore.GetType() };
            }
            finally
            {
                _cache.Clear();
            }
        }
    }
}
