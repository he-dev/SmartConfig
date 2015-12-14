using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartConfig.Data
{
    /// <summary>
    /// Implements the app.configuration as data source.
    /// </summary>
    public class AppConfigSource : DataSource<Setting>
    {
        private IDictionary<Type, IAppConfigSectionHandler> _sectionHandlers;

        public AppConfigSource()
        {
            _sectionHandlers = new object[]
            {
                new AppSettingsSectionHandler(),
                new ConnectionStringsSectionHandler(),
            }
            .Cast<IAppConfigSectionHandler>()
            .ToDictionary(x => x.SectionType, x => x);
        }

        /// <summary>
        /// Gets or sets section handlers.
        /// </summary>
        public IEnumerable<IAppConfigSectionHandler> SectionHandlers
        {
            get { return _sectionHandlers.Values.ToList(); }
            set
            {
                if (value == null || !value.Any())
                {
                    throw new ArgumentNullException(nameof(SectionHandlers), "There must be at least one section handler.");
                }
                _sectionHandlers = value.ToDictionary(x => x.SectionType);
            }
        }

        public IEnumerable<string> SectionNames => _sectionHandlers.Values.Select(sh => sh.SectionName);        

        private ConfigurationSection GetConfigurationSection(System.Configuration.Configuration configuration, SettingKey nameKey)
        {
            Debug.Assert(configuration != null);
            Debug.Assert(nameKey != null);

            var sectionName = GetSectionName(nameKey);
            var actualSectionName = configuration.Sections.Keys.Cast<string>().Single(x => x.Equals(sectionName, StringComparison.OrdinalIgnoreCase));
            var section = configuration.Sections[actualSectionName];
            return section;
        }

        private string GetSectionName(SettingKey nameKey)
        {
            Debug.Assert(nameKey != null);

            //var sectionNames = _sectionHandlers.Select(x => x.Value.SectionName);
            //var sectionNamePattern = @"(?<SectionName>" + string.Join("|", sectionNames) + @")";

            //var sectionNameMatch = Regex.Match(nameKey.Value, sectionNamePattern, RegexOptions.ExplicitCapture);
            //if (!sectionNameMatch.Groups["SectionName"].Success)
            //{
            //throw new InvalidOperationException($"Section name not found in '{nameKey.Value}'");
            //}

            //return sectionNameMatch.Groups["SectionName"].Value;

            var parts = nameKey.Value.Split('.');
            var sectionName = parts.Take(2).First(p => SectionNames.Contains(p));
            return sectionName;
        }

        private string GetSettingName(SettingKey nameKey)
        {
            Debug.Assert(nameKey != null);

            // (?<= AppSettings | ConnectionStrings)\.(?< Key >.+$)

            //var sectionName = GetSectionName(nameKey);
            //var keyPattern = $"(?<={sectionName})\\.(?<Key>.+$)";
            //var keyMatch = Regex.Match(nameKey.Value, keyPattern, RegexOptions.ExplicitCapture);
            //if (!keyMatch.Groups["Key"].Success)
            //{
            //    throw new InvalidOperationException($"Key not found in '{nameKey.Value}'");
            //}

            //return keyMatch.Groups["Key"].Value;

            var parts = nameKey.Value.Split('.');
            var sectionNameIndex = parts.Select((p, i) => new { p, i }).First(x => SectionNames.Contains(x.p)).i;
            var keyParts = parts.Where((p, i) => i != sectionNameIndex);
            return string.Join(".", keyParts);
        }

        public override string Select(IReadOnlyCollection<SettingKey> keys)
        {
            Debug.Assert(keys != null && keys.Any());

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys.First());
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            var settingName = GetSettingName(keys.First());
            var value = sectionHandler.Select(configurationSection, settingName);
            return value;
        }

        public override void Update(IReadOnlyCollection<SettingKey> keys, string value)
        {
            Debug.Assert(keys != null && keys.Any());

            var exeConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = GetConfigurationSection(exeConfig, keys.First());
            var sectionHandler = _sectionHandlers[configurationSection.GetType()];
            sectionHandler.Update(configurationSection, GetSettingName(keys.First()), value);
            exeConfig.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
